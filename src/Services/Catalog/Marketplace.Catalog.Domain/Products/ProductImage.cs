using Marketplace.SharedKernel;

namespace Marketplace.Catalog.Domain.Products;

/// <summary>An image belonging to a product. An entity within the Product aggregate.</summary>
public sealed class ProductImage : Entity<Guid>
{
    private ProductImage(Guid id) : base(id)
    {
    }

    internal ProductImage(Guid id, string url, string? altText, int sortOrder, bool isPrimary) : base(id)
    {
        Url = url;
        AltText = altText;
        SortOrder = sortOrder;
        IsPrimary = isPrimary;
    }

    public string Url { get; private set; } = null!;
    public string? AltText { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsPrimary { get; private set; }

    internal void SetPrimary(bool isPrimary) => IsPrimary = isPrimary;
}
