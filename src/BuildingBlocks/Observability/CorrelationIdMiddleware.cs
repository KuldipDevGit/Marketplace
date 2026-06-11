using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Marketplace.Observability;

/// <summary>
/// Ensures every request carries a correlation id: it reuses an inbound <c>X-Correlation-ID</c>
/// header when present, otherwise falls back to the framework trace identifier. The id is echoed
/// on the response and pushed onto the Serilog log context so all logs for the request carry it.
/// </summary>
public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId =
            context.Request.Headers.TryGetValue(HeaderName, out var headerValue) && !string.IsNullOrWhiteSpace(headerValue)
                ? headerValue.ToString()
                : context.TraceIdentifier;

        context.Response.Headers[HeaderName] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }
}

public static class CorrelationIdMiddlewareExtensions
{
    /// <summary>Propagates a correlation id and enriches logs with it (ADR-0012, API-10).</summary>
    public static IApplicationBuilder UseMarketplaceCorrelationId(this IApplicationBuilder app) =>
        app.UseMiddleware<CorrelationIdMiddleware>();
}
