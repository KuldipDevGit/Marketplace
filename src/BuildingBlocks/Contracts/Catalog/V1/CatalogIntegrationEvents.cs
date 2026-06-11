namespace Marketplace.Contracts.Catalog.V1;

// Versioned, append-only integration events published by the Catalog service (ADR-0008).
// A breaking change introduces a V2 event; these are never edited in place.

public sealed record ProductCreatedIntegrationEvent : IntegrationEvent
{
    public required Guid ProductId { get; init; }
    public required Guid SellerId { get; init; }
    public required Guid CategoryId { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public required string Sku { get; init; }
    public required string Status { get; init; }
}

public sealed record ProductUpdatedIntegrationEvent : IntegrationEvent
{
    public required Guid ProductId { get; init; }
    public required Guid SellerId { get; init; }
    public required Guid CategoryId { get; init; }
    public required string Name { get; init; }
    public required string Sku { get; init; }
}

public sealed record ProductStatusChangedIntegrationEvent : IntegrationEvent
{
    public required Guid ProductId { get; init; }
    public required Guid SellerId { get; init; }
    public required string Status { get; init; }
}

public sealed record ProductDeletedIntegrationEvent : IntegrationEvent
{
    public required Guid ProductId { get; init; }
    public required Guid SellerId { get; init; }
}

public sealed record CategoryCreatedIntegrationEvent : IntegrationEvent
{
    public required Guid CategoryId { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
    public Guid? ParentCategoryId { get; init; }
}

public sealed record CategoryUpdatedIntegrationEvent : IntegrationEvent
{
    public required Guid CategoryId { get; init; }
    public required string Name { get; init; }
    public required string Slug { get; init; }
}
