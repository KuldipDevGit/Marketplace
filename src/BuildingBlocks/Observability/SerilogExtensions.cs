using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Marketplace.Observability;

public static class SerilogExtensions
{
    /// <summary>
    /// Registers Serilog as the logging provider, reading sinks/levels from configuration and DI,
    /// and enriching every log entry with ambient <c>LogContext</c> properties such as CorrelationId
    /// (ADR-0012). Telemetry is also exported via OpenTelemetry in ServiceDefaults.
    /// </summary>
    public static IHostApplicationBuilder AddMarketplaceSerilog(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, configuration) => configuration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

        return builder;
    }
}
