namespace Marketplace.Contracts;

/// <summary>
/// Base type for all cross-service integration events published on the bus (ADR-0008).
/// Concrete events live under a versioned namespace (e.g. <c>Marketplace.Contracts.Catalog.V1</c>)
/// and are append-only: a breaking change introduces a new version, never edits an existing one.
/// </summary>
public abstract record IntegrationEvent
{
    /// <summary>Stable unique id for this event instance (used for idempotent consumption, DB-10).</summary>
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <summary>When the event occurred, in UTC. Set by the publisher from its clock.</summary>
    public DateTime OccurredOnUtc { get; init; }
}
