namespace Marketplace.SharedKernel;

/// <summary>
/// An aggregate root with audit columns, soft-delete, and an optimistic-concurrency token —
/// the standard persisted root for marketplace services (DB-6, DB-7, DB-8). Audit values are set
/// by a persistence interceptor; the concurrency token is managed by EF Core.
/// </summary>
public abstract class AuditableAggregateRoot<TId> : AggregateRoot<TId>, IAuditable, ISoftDeletable
    where TId : notnull
{
    protected AuditableAggregateRoot(TId id) : base(id)
    {
    }

    public DateTime CreatedAtUtc { get; private set; }
    public string? CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public string? UpdatedBy { get; private set; }
    public bool IsDeleted { get; private set; }

    /// <summary>SQL Server rowversion concurrency token (DB-8); managed by EF Core.</summary>
    public byte[] RowVersion { get; private set; } = [];

    public void SetCreated(DateTime utcNow, string? by)
    {
        CreatedAtUtc = utcNow;
        CreatedBy = by;
    }

    public void SetModified(DateTime utcNow, string? by)
    {
        UpdatedAtUtc = utcNow;
        UpdatedBy = by;
    }

    public void MarkDeleted() => IsDeleted = true;
}
