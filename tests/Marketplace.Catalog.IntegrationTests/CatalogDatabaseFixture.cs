using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Infrastructure.Persistence;
using Marketplace.Catalog.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;

namespace Marketplace.Catalog.IntegrationTests;

/// <summary>
/// Spins up a real SQL Server in a container (Testcontainers, TEST-5) and applies the EF migration once,
/// so integration tests exercise the actual persistence stack — including the audit interceptor — not an
/// in-memory substitute. Requires a Docker runtime; runs in CI on Docker-enabled runners.
/// </summary>
public sealed class CatalogDatabaseFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder().Build();

    public CatalogDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseSqlServer(_container.GetConnectionString())
            .AddInterceptors(new AuditableEntityInterceptor(new TestCurrentUser(), TimeProvider.System))
            .Options;
        return new CatalogDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => await _container.DisposeAsync();

    private sealed class TestCurrentUser : ICurrentUser
    {
        public bool IsAuthenticated => true;
        public Guid? UserId { get; } = Guid.NewGuid();
        public Guid? SellerId => null;
        public bool IsAdmin => true;
        public bool IsSeller => false;
    }
}

[CollectionDefinition(Name)]
public sealed class CatalogDatabaseCollection : ICollectionFixture<CatalogDatabaseFixture>
{
    public const string Name = "Catalog database";
}
