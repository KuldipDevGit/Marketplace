using Marketplace.Pricing.Application;
using Marketplace.Pricing.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Aspire defaults: OpenTelemetry, health checks, resilience, service discovery (ADR-0012).
builder.AddServiceDefaults();

// Application + infrastructure composition (Clean Architecture). Controllers, auth, OpenAPI/Scalar
// and the pricing endpoints arrive in the next increment.
builder.Services.AddPricingApplication();
builder.Services.AddPricingInfrastructure();

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();

// Exposed so the functional test project can boot the API via WebApplicationFactory<Program>.
public partial class Program { }
