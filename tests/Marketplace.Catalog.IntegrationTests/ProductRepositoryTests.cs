using FluentAssertions;
using Marketplace.Catalog.Application.Products;
using Marketplace.Catalog.Domain.Products;
using Marketplace.Catalog.Infrastructure.Persistence.Repositories;
using Xunit;

namespace Marketplace.Catalog.IntegrationTests;

[Collection(CatalogDatabaseCollection.Name)]
public class ProductRepositoryTests(CatalogDatabaseFixture fixture)
{
    [Fact]
    public async Task Product_with_images_and_attributes_round_trips()
    {
        var id = Guid.NewGuid();

        await using (var ctx = fixture.CreateContext())
        {
            var product = new Product(id, Guid.NewGuid(), Guid.NewGuid(), null, "Widget", $"widget-{id:N}", "A widget", "SKU-1");
            product.SetAttributes([new ProductAttribute("Color", "Red")]);
            product.AddImage(Guid.NewGuid(), "https://cdn/1.png", "front", 0, isPrimary: true);
            await new ProductRepository(ctx).AddAsync(product);
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = fixture.CreateContext())
        {
            var dto = await new ProductRepository(ctx).GetDtoByIdAsync(id);
            dto.Should().NotBeNull();
            dto!.Attributes.Should().ContainSingle(a => a.Name == "Color" && a.Value == "Red");
            dto.Images.Should().ContainSingle(i => i.IsPrimary && i.Url == "https://cdn/1.png");
            dto.RowVersion.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task List_with_ActiveOnly_excludes_non_active_products()
    {
        var sellerId = Guid.NewGuid();

        await using (var ctx = fixture.CreateContext())
        {
            var repository = new ProductRepository(ctx);
            await repository.AddAsync(new Product(Guid.NewGuid(), sellerId, Guid.NewGuid(), null, "Draft", $"d-{Guid.NewGuid():N}", "d", "S1"));

            var active = new Product(Guid.NewGuid(), sellerId, Guid.NewGuid(), null, "Active", $"a-{Guid.NewGuid():N}", "a", "S2");
            active.ChangeStatus(ProductStatus.Active);
            await repository.AddAsync(active);
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = fixture.CreateContext())
        {
            var filter = new ProductListFilter(null, sellerId, null, null, null, ActiveOnly: true, Page: 1, PageSize: 20, Sort: null);
            var page = await new ProductRepository(ctx).ListAsync(filter);

            page.Items.Should().ContainSingle();
            page.Items.Should().OnlyContain(p => p.Status == ProductStatus.Active);
        }
    }
}
