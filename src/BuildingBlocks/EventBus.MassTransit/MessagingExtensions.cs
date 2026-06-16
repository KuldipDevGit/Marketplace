using Marketplace.Application.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.EventBus;

public static class MessagingExtensions
{
    /// <summary>
    /// Registers MassTransit over RabbitMQ (ADR-0008) with the transactional outbox/inbox bound to
    /// the service's <typeparamref name="TDbContext"/>, so integration events are published in the
    /// same transaction as state changes and consumed with exactly-once effect (DB-9, DB-10).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="rabbitMqConnectionString">AMQP connection string (provided by Aspire as "messaging").</param>
    /// <param name="configure">Optional hook to register this service's consumers and sagas.</param>
    public static IServiceCollection AddMarketplaceMessaging<TDbContext>(
        this IServiceCollection services,
        string rabbitMqConnectionString,
        Action<IBusRegistrationConfigurator>? configure = null)
        where TDbContext : DbContext
    {
        services.AddMassTransit(registration =>
        {
            registration.SetKebabCaseEndpointNameFormatter();

            registration.AddEntityFrameworkOutbox<TDbContext>(outbox =>
            {
                outbox.UseSqlServer();
                outbox.UseBusOutbox();
            });

            configure?.Invoke(registration);

            registration.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(rabbitMqConnectionString));
                cfg.UseMessageRetry(retry => retry.Intervals(
                    TimeSpan.FromMilliseconds(200),
                    TimeSpan.FromMilliseconds(500),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5)));
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        return services;
    }

    /// <summary>
    /// Registers MassTransit on the in-memory transport — no broker required. Used for local
    /// standalone development (no RabbitMQ available); production and Aspire use the RabbitMQ overload
    /// with the transactional outbox.
    /// </summary>
    public static IServiceCollection AddMarketplaceMessagingInMemory(
        this IServiceCollection services,
        Action<IBusRegistrationConfigurator>? configure = null)
    {
        services.AddMassTransit(registration =>
        {
            registration.SetKebabCaseEndpointNameFormatter();
            configure?.Invoke(registration);
            registration.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
        });

        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

        return services;
    }
}
