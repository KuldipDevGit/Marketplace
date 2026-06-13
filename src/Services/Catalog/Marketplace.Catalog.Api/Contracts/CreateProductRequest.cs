namespace Marketplace.Catalog.Api.Contracts;

public sealed record CreateProductRequest(
    Guid CategoryId,
    Guid? BrandId,
    string Name,
    string? Slug,
    string Description,
    string Sku,
    IReadOnlyList<ProductAttributeRequest>? Attributes,
    IReadOnlyList<ProductImageRequest>? Images);
