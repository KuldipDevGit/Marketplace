namespace Marketplace.Catalog.Api.Contracts;

public sealed class UpdateCategoryRequest
{
    public required string Name { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public required int SortOrder { get; set; }
    public required bool IsActive { get; set; }
}
