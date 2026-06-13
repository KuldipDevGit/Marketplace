using FluentAssertions;
using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Domain.Products;
using Marketplace.Catalog.Infrastructure.Persistence;
using Marketplace.Catalog.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;

namespace Marketplace.Catalog.IntegrationTests;

/// <summary>
/// Exercises the development seeder against a fresh SQL Server (its own container, independent of the
/// shared fixture) so the idempotent skip-when-populated behaviour is observable.
/// </summary>
public sealed class CatalogDbSeederTests : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();

    [Fact]
    public async Task Seeding_a_fresh_database_creates_the_demo_catalog()
    {
        await using var context = CreateContext();

        await CatalogDbSeeder.SeedDevelopmentDataAsync(context);

        (await context.Categories.CountAsync()).Should().Be(4);
        (await context.Brands.CountAsync()).Should().Be(2);
        (await context.Products.CountAsync()).Should().Be(4);
        (await context.Products.CountAsync(p => p.Status == ProductStatus.Active)).Should().Be(4);
    }

    [Fact]
    public async Task Seeding_twice_does_not_duplicate_rows()
    {
        await using var context = CreateContext();

        await CatalogDbSeeder.SeedDevelopmentDataAsync(context);
        await CatalogDbSeeder.SeedDevelopmentDataAsync(context);

        (await context.Categories.IgnoreQueryFilters().CountAsync()).Should().Be(4);
        (await context.Brands.IgnoreQueryFilters().CountAsync()).Should().Be(2);
        (await context.Products.IgnoreQueryFilters().CountAsync()).Should().Be(4);
    }

    private CatalogDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseSqlServer(_container.GetConnectionString())
            .AddInterceptors(new AuditableEntityInterceptor(new SeedCurrentUser(), TimeProvider.System))
            .Options;
        return new CatalogDbContext(options);
    }

    /// <summary>The seeder runs outside any request, so there is no authenticated caller.</summary>
    private sealed class SeedCurrentUser : ICurrentUser
    {
        public bool IsAuthenticated => false;
        public Guid? UserId => null;
        public Guid? SellerId => null;
        public bool IsAdmin => false;
        public bool IsSeller => false;
    }
}
