using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);

// Message broker (ADR-0008): RabbitMQ with the management UI; persistent across dev restarts.
var messaging = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

// Relational store (ADR-0005): SQL Server. Each service owns its own database (database-per-service).
var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);
var catalogDb = sql.AddDatabase("catalogdb");

// Catalog service (Phase 2): references its own database and the broker, and waits for both to be ready.
builder.AddProject<Projects.Marketplace_Catalog_Api>("catalog-api")
    .WithReference(catalogDb)
    .WaitFor(catalogDb)
    .WithReference(messaging)
    .WaitFor(messaging);

builder.Build().Run();
