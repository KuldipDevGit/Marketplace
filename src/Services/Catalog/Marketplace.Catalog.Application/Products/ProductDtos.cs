using Marketplace.Catalog.Domain.Products;

namespace Marketplace.Catalog.Application.Products;

public sealed class ProductAttributeDto
{
    public required string Name { get; set; }
    public required string Value { get; set; }
}

public sealed class ProductImageDto
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public string? AltText { get; set; }
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
}

public sealed class ProductSummaryDto
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public ProductStatus Status { get; set; }
}

public sealed class ProductDto
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Description { get; set; }
    public required string Sku { get; set; }
    public ProductStatus Status { get; set; }
    public required IReadOnlyList<ProductAttributeDto> Attributes { get; set; }
    public required IReadOnlyList<ProductImageDto> Images { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public required string RowVersion { get; set; }
}

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
