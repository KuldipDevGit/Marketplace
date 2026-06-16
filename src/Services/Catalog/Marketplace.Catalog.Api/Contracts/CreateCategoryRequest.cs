namespace Marketplace.Catalog.Api.Contracts;

public sealed class CreateCategoryRequest
{
    public required string Name { get; set; }
    public string? Slug { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
