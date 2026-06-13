using FluentAssertions;
using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Application.Categories;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Domain.Categories;
using Marketplace.Contracts;
using NSubstitute;
using Xunit;

namespace Marketplace.Catalog.Application.UnitTests;

public class CategoryHandlerTests
{
    private readonly ICategoryRepository _repository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IIntegrationEventPublisher _publisher = Substitute.For<IIntegrationEventPublisher>();
    private readonly CreateCategoryHandler _handler;

    public CategoryHandlerTests() =>
        _handler = new CreateCategoryHandler(_repository, _unitOfWork, _publisher, TimeProvider.System);

    [Fact]
    public async Task Create_throws_conflict_when_slug_already_exists()
    {
        _repository.SlugExistsAsync("books", null, Arg.Any<CancellationToken>()).Returns(true);

        var act = () => _handler.Handle(new CreateCategoryCommand("Books", "books", null, null, null, 0, true), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Create_throws_not_found_when_parent_does_not_exist()
    {
        var parentId = Guid.NewGuid();
        _repository.SlugExistsAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>()).Returns(false);
        _repository.ExistsAsync(parentId, Arg.Any<CancellationToken>()).Returns(false);

        var act = () => _handler.Handle(new CreateCategoryCommand("Books", "books", parentId, null, null, 0, true), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Create_persists_publishes_and_generates_slug_from_name()
    {
        _repository.SlugExistsAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>()).Returns(false);

        var id = await _handler.Handle(new CreateCategoryCommand("Home & Garden", null, null, null, null, 0, true), CancellationToken.None);

        id.Should().NotBeEmpty();
        await _repository.Received(1).AddAsync(Arg.Is<Category>(c => c.Slug == "home-garden"), Arg.Any<CancellationToken>());
        await _publisher.Received(1).PublishAsync(Arg.Any<IntegrationEvent>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
