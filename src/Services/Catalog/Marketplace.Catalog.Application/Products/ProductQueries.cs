using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Domain.Products;
using MediatR;

namespace Marketplace.Catalog.Application.Products;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

public sealed class GetProductByIdHandler(IProductRepository products, ICurrentUser currentUser)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await products.GetDtoByIdAsync(request.Id, cancellationToken);

        // Treat a product the caller may not see as not-found, so hidden listings are not revealed.
        if (product is null || !ProductAuthorization.CanView(currentUser, product.Status, product.SellerId))
        {
            throw new NotFoundException($"Product '{request.Id}' was not found.");
        }

        return product;
    }
}

public sealed record GetProductBySlugQuery(string Slug) : IRequest<ProductDto>;

public sealed class GetProductBySlugHandler(IProductRepository products, ICurrentUser currentUser)
    : IRequestHandler<GetProductBySlugQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductBySlugQuery request, CancellationToken cancellationToken)
    {
        var product = await products.GetDtoBySlugAsync(request.Slug, cancellationToken);

        if (product is null || !ProductAuthorization.CanView(currentUser, product.Status, product.SellerId))
        {
            throw new NotFoundException($"Product with slug '{request.Slug}' was not found.");
        }

        return product;
    }
}

public sealed record ListProductsQuery(
    Guid? CategoryId,
    Guid? SellerId,
    Guid? BrandId,
    ProductStatus? Status,
    string? Query,
    int Page,
    int PageSize,
    string? Sort) : IRequest<PagedResult<ProductSummaryDto>>;

public sealed class ListProductsHandler(IProductRepository products, ICurrentUser currentUser)
    : IRequestHandler<ListProductsQuery, PagedResult<ProductSummaryDto>>
{
    public Task<PagedResult<ProductSummaryDto>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {
        // Visibility (SEC-6): anonymous/buyer callers see only Active products; a seller browsing their
        // own listings sees any status; admins see everything.
        var activeOnly = true;
        if (currentUser.IsAdmin)
        {
            activeOnly = false;
        }
        else if (currentUser.SellerId is { } sellerId && request.SellerId == sellerId)
        {
            activeOnly = false;
        }

        var filter = new ProductListFilter(
            request.CategoryId,
            request.SellerId,
            request.BrandId,
            request.Status,
            request.Query,
            activeOnly,
            request.Page,
            request.PageSize,
            request.Sort);

        return products.ListAsync(filter, cancellationToken);
    }
}
