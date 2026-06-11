using Marketplace.SharedKernel;

namespace Marketplace.Catalog.Domain.Categories;

/// <summary>A node in the admin-curated category taxonomy; may have a parent for sub-categories (ADR-0013).</summary>
public sealed class Category : AuditableAggregateRoot<Guid>
{
    private Category(Guid id) : base(id)
    {
    }

    public Category(
        Guid id,
        string name,
        string slug,
        Guid? parentCategoryId,
        string? description,
        string? imageUrl,
        int sortOrder,
        bool isActive) : base(id)
    {
        Name = name;
        Slug = slug;
        ParentCategoryId = parentCategoryId;
        Description = description;
        ImageUrl = imageUrl;
        SortOrder = sortOrder;
        IsActive = isActive;
        RaiseDomainEvent(new CategoryCreatedDomainEvent(id, name, slug, parentCategoryId));
    }

    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public Guid? ParentCategoryId { get; private set; }
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    public void Update(string name, Guid? parentCategoryId, string? description, string? imageUrl, int sortOrder, bool isActive)
    {
        Name = name;
        ParentCategoryId = parentCategoryId;
        Description = description;
        ImageUrl = imageUrl;
        SortOrder = sortOrder;
        IsActive = isActive;
        RaiseDomainEvent(new CategoryUpdatedDomainEvent(Id, name, Slug));
    }
}
