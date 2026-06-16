using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Infrastructure.Persistence;
using Marketplace.Catalog.Infrastructure.Persistence.Repositories;
using Marketplace.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<AuditableEntityInterceptor>();

        var catalogDb = configuration.GetConnectionString("catalogdb")
            ?? throw new InvalidOperationException("Connection string 'catalogdb' is not configured.");

        services.AddDbContext<CatalogDbContext>((sp, options) =>
        {
            options.UseSqlServer(catalogDb, sql => sql.MigrationsAssembly(typeof(CatalogDbContext).Assembly.FullName));
            options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CatalogDbContext>());
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        // When a broker connection string is configured (Aspire/production), publish integration
        // events over RabbitMQ with the transactional outbox. For standalone local development no
        // broker is available, so fall back to the in-memory transport — keeping the app fully
        // runnable against LocalDB with no Docker (ADR-0008).
        var messaging = configuration.GetConnectionString("messaging");
        if (string.IsNullOrWhiteSpace(messaging))
        {
            services.AddMarketplaceMessagingInMemory();
        }
        else
        {
            services.AddMarketplaceMessaging<CatalogDbContext>(messaging);
        }

        return services;
    }
}
