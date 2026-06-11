using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);

// Message broker (ADR-0008): RabbitMQ with the management UI.
// Persistent lifetime keeps the container (and queues) across dev restarts.
builder.AddRabbitMQ("messaging")
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

// Relational store (ADR-0005): SQL Server. Each service owns its own database,
// added here alongside the service project from Phase 2 onward (database-per-service).
builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

builder.Build().Run();
