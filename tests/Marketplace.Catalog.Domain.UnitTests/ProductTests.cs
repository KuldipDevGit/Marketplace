using FluentAssertions;
using Marketplace.Catalog.Domain.Products;
using Xunit;

namespace Marketplace.Catalog.Domain.UnitTests;

public class ProductTests
{
    private static Product NewProduct() => new(
        Guid.NewGuid(),
        sellerId: Guid.NewGuid(),
        categoryId: Guid.NewGuid(),
        brandId: null,
        name: "Widget",
        slug: "widget",
        description: "A useful widget",
        sku: "SKU-1");

    [Fact]
    public void New_product_starts_in_Draft_and_raises_created_event()
    {
        var product = NewProduct();

        product.Status.Should().Be(ProductStatus.Draft);
        product.DomainEvents.Should().ContainSingle(e => e is ProductCreatedDomainEvent);
    }

    [Fact]
    public void Changing_status_to_the_same_value_raises_no_event()
    {
        var product = NewProduct();
        product.ClearDomainEvents();

        product.ChangeStatus(ProductStatus.Draft);

        product.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Changing_status_updates_state_and_raises_event()
    {
        var product = NewProduct();
        product.ClearDomainEvents();

        product.ChangeStatus(ProductStatus.Active);

        product.Status.Should().Be(ProductStatus.Active);
        product.DomainEvents.Should().ContainSingle(e => e is ProductStatusChangedDomainEvent);
    }

    [Fact]
    public void First_image_is_primary_even_when_not_requested()
    {
        var product = NewProduct();

        var image = product.AddImage(Guid.NewGuid(), "https://cdn/img1.png", altText: null, sortOrder: 0, isPrimary: false);

        image.IsPrimary.Should().BeTrue();
    }

    [Fact]
    public void Adding_a_primary_image_demotes_the_previous_primary()
    {
        var product = NewProduct();
        var first = product.AddImage(Guid.NewGuid(), "https://cdn/img1.png", null, 0, isPrimary: true);
        var second = product.AddImage(Guid.NewGuid(), "https://cdn/img2.png", null, 1, isPrimary: true);

        first.IsPrimary.Should().BeFalse();
        second.IsPrimary.Should().BeTrue();
        product.Images.Count(i => i.IsPrimary).Should().Be(1);
    }

    [Fact]
    public void Removing_the_primary_image_promotes_another()
    {
        var product = NewProduct();
        var first = product.AddImage(Guid.NewGuid(), "https://cdn/img1.png", null, 0, isPrimary: true);
        product.AddImage(Guid.NewGuid(), "https://cdn/img2.png", null, 1, isPrimary: false);

        product.RemoveImage(first.Id);

        product.Images.Should().ContainSingle();
        product.Images[0].IsPrimary.Should().BeTrue();
    }

    [Fact]
    public void Delete_marks_deleted_and_raises_event()
    {
        var product = NewProduct();
        product.ClearDomainEvents();

        product.Delete();

        product.IsDeleted.Should().BeTrue();
        product.DomainEvents.Should().ContainSingle(e => e is ProductDeletedDomainEvent);
    }

    [Fact]
    public void UpdateDetails_replaces_attributes_and_raises_event()
    {
        var product = NewProduct();
        product.ClearDomainEvents();

        product.UpdateDetails(
            categoryId: Guid.NewGuid(),
            brandId: null,
            name: "Better widget",
            description: "Now improved",
            sku: "SKU-2",
            attributes: [new ProductAttribute("Color", "Red")]);

        product.Name.Should().Be("Better widget");
        product.Attributes.Should().ContainSingle(a => a.Name == "Color" && a.Value == "Red");
        product.DomainEvents.Should().ContainSingle(e => e is ProductUpdatedDomainEvent);
    }
}
