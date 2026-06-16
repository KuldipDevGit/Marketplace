using System.Reflection;
using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Domain.Brands;
using Marketplace.Catalog.Domain.Categories;
using Marketplace.Catalog.Domain.Products;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Catalog.Infrastructure.Persistence;

/// <summary>The Catalog service's own database (database-per-service, DB-1). Also the unit of work (BE-4).</summary>
public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public const string Schema = "catalog";

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Brand> Brands => Set<Brand>();

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // MassTransit transactional outbox + inbox tables (ADR-0008, DB-9/DB-10).
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        base.OnModelCreating(modelBuilder);
    }
}
