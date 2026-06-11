using Marketplace.SharedKernel;

namespace Marketplace.Catalog.Domain.Products;

/// <summary>
/// A seller-owned catalog product (ADR-0013). Owns its images and attributes. Price and stock are
/// out of scope (Pricing/Inventory). New products start in <see cref="ProductStatus.Draft"/>.
/// </summary>
public sealed class Product : AuditableAggregateRoot<Guid>
{
    private readonly List<ProductImage> _images = [];
    private readonly List<ProductAttribute> _attributes = [];

    private Product(Guid id) : base(id)
    {
    }

    public Product(
        Guid id,
        Guid sellerId,
        Guid categoryId,
        Guid? brandId,
        string name,
        string slug,
        string description,
        string sku) : base(id)
    {
        SellerId = sellerId;
        CategoryId = categoryId;
        BrandId = brandId;
        Name = name;
        Slug = slug;
        Description = description;
        Sku = sku;
        Status = ProductStatus.Draft;
        RaiseDomainEvent(new ProductCreatedDomainEvent(id, sellerId, categoryId, name, slug, sku));
    }

    public Guid SellerId { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid? BrandId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Sku { get; private set; } = null!;
    public ProductStatus Status { get; private set; }

    public IReadOnlyList<ProductImage> Images => _images.AsReadOnly();
    public IReadOnlyList<ProductAttribute> Attributes => _attributes.AsReadOnly();

    public void UpdateDetails(
        Guid categoryId,
        Guid? brandId,
        string name,
        string description,
        string sku,
        IEnumerable<ProductAttribute> attributes)
    {
        CategoryId = categoryId;
        BrandId = brandId;
        Name = name;
        Description = description;
        Sku = sku;
        _attributes.Clear();
        _attributes.AddRange(attributes);
        RaiseDomainEvent(new ProductUpdatedDomainEvent(Id, SellerId, categoryId, name, sku));
    }

    public void ChangeStatus(ProductStatus status)
    {
        if (Status == status)
        {
            return;
        }

        Status = status;
        RaiseDomainEvent(new ProductStatusChangedDomainEvent(Id, SellerId, status));
    }

    public ProductImage AddImage(Guid imageId, string url, string? altText, int sortOrder, bool isPrimary)
    {
        var makePrimary = isPrimary || _images.Count == 0;
        if (makePrimary)
        {
            foreach (var existing in _images)
            {
                existing.SetPrimary(false);
            }
        }

        var image = new ProductImage(imageId, url, altText, sortOrder, makePrimary);
        _images.Add(image);
        return image;
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image is null)
        {
            return;
        }

        _images.Remove(image);
        if (image.IsPrimary && _images.Count > 0)
        {
            _images[0].SetPrimary(true);
        }
    }

    public void Delete()
    {
        MarkDeleted();
        RaiseDomainEvent(new ProductDeletedDomainEvent(Id, SellerId));
    }
}
