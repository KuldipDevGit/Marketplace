using Marketplace.Catalog.Domain.Products;

namespace Marketplace.Catalog.Api.Contracts;

// Request payloads bound from the body. They mirror the OpenAPI contract (openapi/catalog.v1.yaml)
// and are mapped to MediatR commands in the controllers.

public sealed record CreateCategoryRequest(
    string Name,
    string? Slug,
    Guid? ParentCategoryId,
    string? Description,
    string? ImageUrl,
    int SortOrder = 0,
    bool IsActive = true);

public sealed record UpdateCategoryRequest(
    string Name,
    Guid? ParentCategoryId,
    string? Description,
    string? ImageUrl,
    int SortOrder,
    bool IsActive);

public sealed record CreateBrandRequest(string Name, string? Slug, string? LogoUrl, bool IsActive = true);

public sealed record UpdateBrandRequest(string Name, string? LogoUrl, bool IsActive);

public sealed record ProductAttributeRequest(string Name, string Value);

public sealed record ProductImageRequest(string Url, string? AltText, int SortOrder = 0, bool IsPrimary = false);

public sealed record CreateProductRequest(
    Guid CategoryId,
    Guid? BrandId,
    string Name,
    string? Slug,
    string Description,
    string Sku,
    IReadOnlyList<ProductAttributeRequest>? Attributes,
    IReadOnlyList<ProductImageRequest>? Images);

public sealed record UpdateProductRequest(
    Guid CategoryId,
    Guid? BrandId,
    string Name,
    string Description,
    string Sku,
    IReadOnlyList<ProductAttributeRequest>? Attributes,
    string RowVersion);

public sealed record UpdateProductStatusRequest(ProductStatus Status, string RowVersion);

public sealed record AddProductImageRequest(string Url, string? AltText, int SortOrder = 0, bool IsPrimary = false);
