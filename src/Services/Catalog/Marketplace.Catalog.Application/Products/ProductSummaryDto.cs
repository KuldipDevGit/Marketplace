using Marketplace.Catalog.Domain.Products;

namespace Marketplace.Catalog.Application.Products;

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
