namespace Marketplace.Catalog.Api.Contracts;

public sealed record UpdateProductRequest(
    Guid CategoryId,
    Guid? BrandId,
    string Name,
    string Description,
    string Sku,
    IReadOnlyList<ProductAttributeRequest>? Attributes,
    string RowVersion);
