using FluentAssertions;
using Marketplace.Catalog.Infrastructure;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Marketplace.Catalog.IntegrationTests;

/// <summary>
/// Verifies the messaging transport is chosen from configuration — no broker or SQL Server required,
/// so these run without Docker. A "messaging" connection string selects RabbitMQ (with the
/// transactional outbox); its absence falls back to the in-memory transport so the service runs
/// standalone locally without a broker (ADR-0008).
/// </summary>
public class MessagingRegistrationTests
{
    private static ServiceProvider BuildProvider(bool withBroker)
    {
        var settings = new Dictionary<string, string?>
        {
            // Never connected to — only required so AddCatalogInfrastructure can register the DbContext.
            ["ConnectionStrings:catalogdb"] = "Server=(localdb)\\MSSQLLocalDB;Database=unused;Trusted_Connection=True",
        };

        if (withBroker)
        {
            settings["ConnectionStrings:messaging"] = "amqp://guest:guest@localhost:5672";
        }

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCatalogInfrastructure(configuration);
        return services.BuildServiceProvider();
    }

    [Fact]
    public void Without_a_broker_connection_string_the_in_memory_transport_is_used()
    {
        using var provider = BuildProvider(withBroker: false);

        var bus = provider.GetRequiredService<IBusControl>();

        // The in-memory transport exposes a loopback:// bus address; building it requires no broker.
        bus.Address.Scheme.Should().Be("loopback");
    }

    [Fact]
    public void With_a_broker_connection_string_the_rabbitmq_transport_is_used()
    {
        using var provider = BuildProvider(withBroker: true);

        var bus = provider.GetRequiredService<IBusControl>();

        // The bus is configured (not started) here — the rabbitmq:// scheme proves the transport
        // selection without opening a connection to the broker.
        bus.Address.Scheme.Should().Be("rabbitmq");
    }
}
