namespace Marketplace.Catalog.Api.Contracts;

public sealed record UpdateCategoryRequest(
    string Name,
    Guid? ParentCategoryId,
    string? Description,
    string? ImageUrl,
    int SortOrder,
    bool IsActive);
