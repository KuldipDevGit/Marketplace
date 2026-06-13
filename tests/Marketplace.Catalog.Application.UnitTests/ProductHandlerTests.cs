using FluentAssertions;
using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Application.Products;
using Marketplace.Catalog.Domain.Products;
using Marketplace.Contracts;
using NSubstitute;
using Xunit;

namespace Marketplace.Catalog.Application.UnitTests;

public class CreateProductHandlerTests
{
    private readonly IProductRepository _products = Substitute.For<IProductRepository>();
    private readonly ICategoryRepository _categories = Substitute.For<ICategoryRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IIntegrationEventPublisher _publisher = Substitute.For<IIntegrationEventPublisher>();
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests() =>
        _handler = new CreateProductHandler(_products, _categories, _currentUser, _unitOfWork, _publisher, TimeProvider.System);

    private static CreateProductCommand Command(Guid categoryId) =>
        new(categoryId, null, "Widget", null, "A widget", "SKU-1", [], []);

    [Fact]
    public async Task Throws_forbidden_when_caller_is_not_a_seller()
    {
        _currentUser.SellerId.Returns((Guid?)null);

        var act = () => _handler.Handle(Command(Guid.NewGuid()), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Throws_not_found_when_category_does_not_exist()
    {
        var categoryId = Guid.NewGuid();
        _currentUser.SellerId.Returns(Guid.NewGuid());
        _categories.ExistsAsync(categoryId, Arg.Any<CancellationToken>()).Returns(false);

        var act = () => _handler.Handle(Command(categoryId), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Creates_product_owned_by_the_current_seller()
    {
        var sellerId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        _currentUser.SellerId.Returns(sellerId);
        _categories.ExistsAsync(categoryId, Arg.Any<CancellationToken>()).Returns(true);
        _products.SlugExistsAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>()).Returns(false);

        var id = await _handler.Handle(Command(categoryId), CancellationToken.None);

        id.Should().NotBeEmpty();
        await _products.Received(1).AddAsync(Arg.Is<Product>(p => p.SellerId == sellerId), Arg.Any<CancellationToken>());
        await _publisher.Received(1).PublishAsync(Arg.Any<IntegrationEvent>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

public class DeleteProductHandlerTests
{
    private readonly IProductRepository _products = Substitute.For<IProductRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IIntegrationEventPublisher _publisher = Substitute.For<IIntegrationEventPublisher>();
    private readonly DeleteProductHandler _handler;

    public DeleteProductHandlerTests() =>
        _handler = new DeleteProductHandler(_products, _currentUser, _unitOfWork, _publisher, TimeProvider.System);

    private static Product ProductOwnedBy(Guid sellerId) =>
        new(Guid.NewGuid(), sellerId, Guid.NewGuid(), null, "Widget", "widget", "A widget", "SKU-1");

    [Fact]
    public async Task A_seller_cannot_delete_another_sellers_product()
    {
        var product = ProductOwnedBy(Guid.NewGuid());
        _products.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _currentUser.IsAdmin.Returns(false);
        _currentUser.SellerId.Returns(Guid.NewGuid()); // a different seller

        var act = () => _handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task An_admin_can_delete_any_product()
    {
        var product = ProductOwnedBy(Guid.NewGuid());
        _products.GetByIdAsync(product.Id, Arg.Any<CancellationToken>()).Returns(product);
        _currentUser.IsAdmin.Returns(true);

        await _handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        product.IsDeleted.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
