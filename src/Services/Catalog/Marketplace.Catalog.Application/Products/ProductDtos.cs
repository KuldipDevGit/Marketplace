using Marketplace.Catalog.Domain.Products;

namespace Marketplace.Catalog.Application.Products;

public sealed record ProductAttributeDto(string Name, string Value);

public sealed record ProductImageDto(Guid Id, string Url, string? AltText, int SortOrder, bool IsPrimary);

public sealed record ProductSummaryDto(
    Guid Id,
    Guid SellerId,
    string Name,
    string Slug,
    Guid CategoryId,
    Guid? BrandId,
    string? PrimaryImageUrl,
    ProductStatus Status);

public sealed record ProductDto(
    Guid Id,
    Guid SellerId,
    Guid CategoryId,
    Guid? BrandId,
    string Name,
    string Slug,
    string Description,
    string Sku,
    ProductStatus Status,
    IReadOnlyList<ProductAttributeDto> Attributes,
    IReadOnlyList<ProductImageDto> Images,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    string RowVersion);

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
