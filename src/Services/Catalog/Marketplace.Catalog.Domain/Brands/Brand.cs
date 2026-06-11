using Marketplace.SharedKernel;

namespace Marketplace.Catalog.Domain.Brands;

/// <summary>A product brand, managed by admins.</summary>
public sealed class Brand : AuditableAggregateRoot<Guid>
{
    private Brand(Guid id) : base(id)
    {
    }

    public Brand(Guid id, string name, string slug, string? logoUrl, bool isActive) : base(id)
    {
        Name = name;
        Slug = slug;
        LogoUrl = logoUrl;
        IsActive = isActive;
    }

    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string? LogoUrl { get; private set; }
    public bool IsActive { get; private set; }

    public void Update(string name, string? logoUrl, bool isActive)
    {
        Name = name;
        LogoUrl = logoUrl;
        IsActive = isActive;
    }
}
