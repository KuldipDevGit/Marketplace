namespace Marketplace.SharedKernel;

/// <summary>
/// An aggregate root — the only entry point for mutating an aggregate and the unit of
/// transactional consistency. Other aggregates are referenced by id, not navigation (BE-9).
/// </summary>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    protected AggregateRoot(TId id) : base(id)
    {
    }
}
