using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Marketplace.Catalog.Infrastructure.Persistence;

/// <summary>
/// Design-time factory so <c>dotnet ef</c> can build the model and generate migrations without the
/// application host. The connection string is a placeholder — it is not used to generate migrations.
/// </summary>
public sealed class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseSqlServer(
                "Server=localhost;Database=catalog;Trusted_Connection=True;TrustServerCertificate=True",
                sql => sql.MigrationsAssembly(typeof(CatalogDbContext).Assembly.FullName))
            .Options;

        return new CatalogDbContext(options);
    }
}
