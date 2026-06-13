using Marketplace.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Catalog.Api.Infrastructure;

internal static class DatabaseMigrationExtensions
{
    /// <summary>
    /// Applies pending migrations at startup. Development convenience only — production applies
    /// migrations as a separate, gated step in the deployment pipeline (INF-11).
    /// </summary>
    public static async Task MigrateCatalogDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
