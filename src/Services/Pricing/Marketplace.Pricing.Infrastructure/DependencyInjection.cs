using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Pricing.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registers Pricing infrastructure services. EF Core persistence (pricingdb) and the MassTransit
    /// transactional outbox are added in the next increment alongside the use cases.
    /// </summary>
    public static IServiceCollection AddPricingInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
