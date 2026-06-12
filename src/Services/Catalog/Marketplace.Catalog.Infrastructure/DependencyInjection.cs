using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Infrastructure.Messaging;
using Marketplace.Catalog.Infrastructure.Persistence;
using Marketplace.Catalog.Infrastructure.Persistence.Interceptors;
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
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        var messaging = configuration.GetConnectionString("messaging")
            ?? throw new InvalidOperationException("Connection string 'messaging' is not configured.");
        services.AddMarketplaceMessaging<CatalogDbContext>(messaging);

        return services;
    }
}
