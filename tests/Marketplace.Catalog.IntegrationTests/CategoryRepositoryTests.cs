using FluentAssertions;
using Marketplace.Catalog.Domain.Categories;
using Marketplace.Catalog.Infrastructure.Persistence.Repositories;
using Xunit;

namespace Marketplace.Catalog.IntegrationTests;

[Collection(CatalogDatabaseCollection.Name)]
public class CategoryRepositoryTests(CatalogDatabaseFixture fixture)
{
    [Fact]
    public async Task Add_then_get_round_trips_through_real_sql()
    {
        var id = Guid.NewGuid();

        await using (var ctx = fixture.CreateContext())
        {
            var repository = new CategoryRepository(ctx);
            await repository.AddAsync(new Category(id, "Books", $"books-{id:N}", null, "All books", null, 1, true));
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = fixture.CreateContext())
        {
            var dto = await new CategoryRepository(ctx).GetDtoByIdAsync(id);
            dto.Should().NotBeNull();
            dto!.Name.Should().Be("Books");
            dto.CreatedAtUtc.Should().NotBe(default);
        }
    }

    [Fact]
    public async Task Soft_deleted_category_is_excluded_by_the_global_query_filter()
    {
        var id = Guid.NewGuid();

        await using (var ctx = fixture.CreateContext())
        {
            var repository = new CategoryRepository(ctx);
            var category = new Category(id, "Temp", $"temp-{id:N}", null, null, null, 0, true);
            await repository.AddAsync(category);
            await ctx.SaveChangesAsync();

            category.MarkDeleted();
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = fixture.CreateContext())
        {
            (await new CategoryRepository(ctx).GetByIdAsync(id)).Should().BeNull();
        }
    }

    [Fact]
    public async Task SlugExists_detects_an_existing_slug()
    {
        var slug = $"unique-{Guid.NewGuid():N}";

        await using (var ctx = fixture.CreateContext())
        {
            await new CategoryRepository(ctx).AddAsync(new Category(Guid.NewGuid(), "Cat", slug, null, null, null, 0, true));
            await ctx.SaveChangesAsync();
        }

        await using (var ctx = fixture.CreateContext())
        {
            (await new CategoryRepository(ctx).SlugExistsAsync(slug)).Should().BeTrue();
            (await new CategoryRepository(ctx).SlugExistsAsync($"absent-{Guid.NewGuid():N}")).Should().BeFalse();
        }
    }
}
