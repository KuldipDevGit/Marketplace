namespace Marketplace.Catalog.Api.Contracts;

public sealed class UpdateProductRequest
{
    public required Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Sku { get; set; }
    public IReadOnlyList<ProductAttributeRequest>? Attributes { get; set; }
    public required string RowVersion { get; set; }
}
