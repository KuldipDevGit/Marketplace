using Marketplace.SharedKernel;

namespace Marketplace.Catalog.Domain.Products;

/// <summary>A free-form product attribute (e.g. Color = Red). A value object owned by the Product.</summary>
public sealed class ProductAttribute : ValueObject
{
    private ProductAttribute()
    {
    }

    public ProductAttribute(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; private set; } = null!;
    public string Value { get; private set; } = null!;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Value;
    }
}
