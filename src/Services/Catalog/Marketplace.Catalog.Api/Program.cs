using System.Text.Json.Serialization;
using Marketplace.Catalog.Api.Infrastructure;
using Marketplace.Catalog.Application;
using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Infrastructure;
using Marketplace.Observability;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Aspire defaults: OpenTelemetry, health checks, resilience, service discovery (ADR-0012).
builder.AddServiceDefaults();
builder.AddMarketplaceSerilog();

// Application + infrastructure composition (Clean Architecture).
builder.Services.AddCatalogApplication();
builder.Services.AddCatalogInfrastructure(builder.Configuration);

// The authenticated caller, projected from JWT claims (ADR-0006, SEC-6).
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// RFC 7807 error responses (API-4).
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddOpenApi();

// Authentication via Microsoft Entra External ID; configuration is environment-supplied (ADR-0006).
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        builder.Configuration.GetSection("Authentication:Schemes:Bearer").Bind(options);
        options.TokenValidationParameters.ValidateAudience = !string.IsNullOrWhiteSpace(options.Audience);
        options.TokenValidationParameters.ValidateIssuer = !string.IsNullOrWhiteSpace(options.Authority);
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();
app.UseMarketplaceCorrelationId();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Interactive API reference (Scalar) at /scalar, with the site root redirecting to it.
    // Development only — the service is a headless JSON API in production (API-1).
    app.MapScalarApiReference();
    app.MapGet("/", () => Results.Redirect("/scalar")).ExcludeFromDescription();

    await app.MigrateCatalogDatabaseAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();
