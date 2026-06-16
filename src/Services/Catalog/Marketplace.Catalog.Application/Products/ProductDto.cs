using Marketplace.Catalog.Domain.Products;

namespace Marketplace.Catalog.Application.Products;

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
