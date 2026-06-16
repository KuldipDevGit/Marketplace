namespace Marketplace.Application.Abstractions;

/// <summary>Commits the current unit of work, persisting tracked changes and flushing the outbox (BE-4).</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
