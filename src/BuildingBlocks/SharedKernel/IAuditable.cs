namespace Marketplace.SharedKernel;

/// <summary>Carries audit metadata populated by the persistence layer (DB-6).</summary>
public interface IAuditable
{
    DateTime CreatedAtUtc { get; }
    string? CreatedBy { get; }
    DateTime? UpdatedAtUtc { get; }
    string? UpdatedBy { get; }

    void SetCreated(DateTime utcNow, string? by);
    void SetModified(DateTime utcNow, string? by);
}

/// <summary>Marks an aggregate as soft-deletable rather than physically removed (DB-7).</summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; }

    void MarkDeleted();
}
