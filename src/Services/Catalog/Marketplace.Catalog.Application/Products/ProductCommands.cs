using FluentValidation;
using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Domain.Products;
using Marketplace.Contracts.Catalog.V1;
using MediatR;

namespace Marketplace.Catalog.Application.Products;

public sealed record NewProductImage(string Url, string? AltText, int SortOrder, bool IsPrimary);

// ----- Create -----

public sealed record CreateProductCommand(
    Guid CategoryId,
    Guid? BrandId,
    string Name,
    string? Slug,
    string Description,
    string Sku,
    IReadOnlyList<ProductAttributeDto> Attributes,
    IReadOnlyList<NewProductImage> Images) : IRequest<Guid>;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(64);
        RuleForEach(x => x.Attributes).ChildRules(a =>
        {
            a.RuleFor(p => p.Name).NotEmpty().MaximumLength(100);
            a.RuleFor(p => p.Value).NotEmpty().MaximumLength(500);
        });
        RuleForEach(x => x.Images).ChildRules(i => i.RuleFor(p => p.Url).NotEmpty().MaximumLength(2048));
    }
}

public sealed class CreateProductHandler(
    IProductRepository products,
    ICategoryRepository categories,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    TimeProvider clock) : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var sellerId = currentUser.SellerId ?? throw new ForbiddenException("Only sellers can create products.");

        if (!await categories.ExistsAsync(request.CategoryId, cancellationToken))
        {
            throw new NotFoundException($"Category '{request.CategoryId}' was not found.");
        }

        var slug = Slug.From(string.IsNullOrWhiteSpace(request.Slug) ? request.Name : request.Slug);
        if (await products.SlugExistsAsync(slug, null, cancellationToken))
        {
            throw new ConflictException($"A product with slug '{slug}' already exists.");
        }

        var product = new Product(Guid.NewGuid(), sellerId, request.CategoryId, request.BrandId, request.Name, slug, request.Description, request.Sku);
        product.SetAttributes(request.Attributes.Select(a => new ProductAttribute(a.Name, a.Value)));
        foreach (var image in request.Images)
        {
            product.AddImage(Guid.NewGuid(), image.Url, image.AltText, image.SortOrder, image.IsPrimary);
        }

        await products.AddAsync(product, cancellationToken);
        await publisher.PublishAsync(
            new ProductCreatedIntegrationEvent
            {
                ProductId = product.Id,
                SellerId = sellerId,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Slug = product.Slug,
                Sku = product.Sku,
                Status = product.Status.ToString(),
                OccurredOnUtc = clock.GetUtcNow().UtcDateTime,
            },
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}

// ----- Update -----

public sealed record UpdateProductCommand(
    Guid Id,
    Guid CategoryId,
    Guid? BrandId,
    string Name,
    string Description,
    string Sku,
    IReadOnlyList<ProductAttributeDto> Attributes,
    byte[] RowVersion) : IRequest;

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(64);
        RuleFor(x => x.RowVersion).NotEmpty();
    }
}

public sealed class UpdateProductHandler(
    IProductRepository products,
    ICategoryRepository categories,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    TimeProvider clock) : IRequestHandler<UpdateProductCommand>
{
    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await ProductCommandGuards.LoadModifiableAsync(products, currentUser, request.Id, cancellationToken);

        if (request.CategoryId != product.CategoryId && !await categories.ExistsAsync(request.CategoryId, cancellationToken))
        {
            throw new NotFoundException($"Category '{request.CategoryId}' was not found.");
        }

        products.SetOriginalRowVersion(product, request.RowVersion);
        product.UpdateDetails(
            request.CategoryId, request.BrandId, request.Name, request.Description, request.Sku,
            request.Attributes.Select(a => new ProductAttribute(a.Name, a.Value)));
        await publisher.PublishAsync(ProductCommandGuards.ProductUpdated(product, clock), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

// ----- Change status -----

public sealed record UpdateProductStatusCommand(Guid Id, ProductStatus Status, byte[] RowVersion) : IRequest;

public sealed class UpdateProductStatusValidator : AbstractValidator<UpdateProductStatusCommand>
{
    public UpdateProductStatusValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.RowVersion).NotEmpty();
    }
}

public sealed class UpdateProductStatusHandler(
    IProductRepository products,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    TimeProvider clock) : IRequestHandler<UpdateProductStatusCommand>
{
    public async Task Handle(UpdateProductStatusCommand request, CancellationToken cancellationToken)
    {
        var product = await ProductCommandGuards.LoadModifiableAsync(products, currentUser, request.Id, cancellationToken);

        products.SetOriginalRowVersion(product, request.RowVersion);
        product.ChangeStatus(request.Status);
        await publisher.PublishAsync(
            new ProductStatusChangedIntegrationEvent
            {
                ProductId = product.Id,
                SellerId = product.SellerId,
                Status = product.Status.ToString(),
                OccurredOnUtc = clock.GetUtcNow().UtcDateTime,
            },
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

// ----- Delete -----

public sealed record DeleteProductCommand(Guid Id) : IRequest;

public sealed class DeleteProductHandler(
    IProductRepository products,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    TimeProvider clock) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await ProductCommandGuards.LoadModifiableAsync(products, currentUser, request.Id, cancellationToken);

        product.Delete();
        await publisher.PublishAsync(
            new ProductDeletedIntegrationEvent
            {
                ProductId = product.Id,
                SellerId = product.SellerId,
                OccurredOnUtc = clock.GetUtcNow().UtcDateTime,
            },
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

// ----- Add image -----

public sealed record AddProductImageCommand(Guid ProductId, string Url, string? AltText, int SortOrder, bool IsPrimary) : IRequest<Guid>;

public sealed class AddProductImageValidator : AbstractValidator<AddProductImageCommand>
{
    public AddProductImageValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Url).NotEmpty().MaximumLength(2048);
        RuleFor(x => x.AltText).MaximumLength(250);
    }
}

public sealed class AddProductImageHandler(
    IProductRepository products,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    TimeProvider clock) : IRequestHandler<AddProductImageCommand, Guid>
{
    public async Task<Guid> Handle(AddProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await ProductCommandGuards.LoadModifiableAsync(products, currentUser, request.ProductId, cancellationToken);

        var image = product.AddImage(Guid.NewGuid(), request.Url, request.AltText, request.SortOrder, request.IsPrimary);
        // An image change alters the product's display (e.g. its primary image); signal a product update
        // so read models stay current, consistent with the other product mutations.
        await publisher.PublishAsync(ProductCommandGuards.ProductUpdated(product, clock), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return image.Id;
    }
}

// ----- Remove image -----

public sealed record RemoveProductImageCommand(Guid ProductId, Guid ImageId) : IRequest;

public sealed class RemoveProductImageHandler(
    IProductRepository products,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork,
    IIntegrationEventPublisher publisher,
    TimeProvider clock) : IRequestHandler<RemoveProductImageCommand>
{
    public async Task Handle(RemoveProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await ProductCommandGuards.LoadModifiableAsync(products, currentUser, request.ProductId, cancellationToken);

        product.RemoveImage(request.ImageId);
        await publisher.PublishAsync(ProductCommandGuards.ProductUpdated(product, clock), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

/// <summary>Shared guards/builders for the product write handlers, so the load-authorize preamble lives once.</summary>
internal static class ProductCommandGuards
{
    /// <summary>Loads a product for a write command, throwing if it is missing or the caller may not modify it.</summary>
    public static async Task<Product> LoadModifiableAsync(
        IProductRepository products,
        ICurrentUser currentUser,
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product '{id}' was not found.");
        ProductAuthorization.EnsureCanModify(currentUser, product);
        return product;
    }

    /// <summary>Builds the "product updated" integration event from the current product state.</summary>
    public static ProductUpdatedIntegrationEvent ProductUpdated(Product product, TimeProvider clock) => new()
    {
        ProductId = product.Id,
        SellerId = product.SellerId,
        CategoryId = product.CategoryId,
        Name = product.Name,
        Sku = product.Sku,
        OccurredOnUtc = clock.GetUtcNow().UtcDateTime,
    };
}
