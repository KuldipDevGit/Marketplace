using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Marketplace.Catalog.FunctionalTests;

/// <summary>
/// Boots the Catalog API in the "Testing" environment — so the Development-only DB migrate/seed is
/// skipped and no SQL Server is needed — and injects a known JWT signing key via the same config
/// schema <c>dotnet user-jwts</c> uses. That lets the auth pipeline (401/403) be asserted without a
/// database: those responses are produced before any handler/repository runs.
/// </summary>
public sealed class CatalogApiFactory : WebApplicationFactory<Program>
{
    public static readonly byte[] SigningKey = Encoding.UTF8.GetBytes("marketplace-functional-tests-signing-key-0001!!");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // UseSetting feeds host configuration, which WebApplication.CreateBuilder reads early — before
        // Program composes the infrastructure — so these are visible to AddCatalogInfrastructure and
        // the JwtBearer setup. The catalog DB is registered but never connected to (401/403 never
        // reach the handler); the signing key matches the one the tests sign their tokens with.
        builder.UseSetting("ConnectionStrings:catalogdb", "Server=(localdb)\\unused;Database=unused;Trusted_Connection=True");
        builder.UseSetting("Authentication:Schemes:Bearer:SigningKeys:0:Value", Convert.ToBase64String(SigningKey));
    }
}
