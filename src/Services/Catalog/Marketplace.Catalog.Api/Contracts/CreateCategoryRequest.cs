namespace Marketplace.Catalog.Api.Contracts;

public sealed record CreateCategoryRequest(
    string Name,
    string? Slug,
    Guid? ParentCategoryId,
    string? Description,
    string? ImageUrl,
    int SortOrder = 0,
    bool IsActive = true);
