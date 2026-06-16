using Marketplace.Catalog.Domain.Products;

namespace Marketplace.Catalog.Application.Products;

/// <summary>Filter + paging options for listing products. Visibility is resolved by the handler from the caller (SEC-6).</summary>
public sealed record ProductListFilter(
    Guid? CategoryId,
    Guid? SellerId,
    Guid? BrandId,
    ProductStatus? Status,
    string? Query,
    bool ActiveOnly,
    int Page,
    int PageSize,
    string? Sort);
