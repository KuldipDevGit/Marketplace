using Marketplace.Catalog.Application.Abstractions;
using Marketplace.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Marketplace.Catalog.Infrastructure.Persistence.Interceptors;

/// <summary>Populates audit columns on auditable aggregates as they are saved (DB-6).</summary>
public sealed class AuditableEntityInterceptor(ICurrentUser currentUser, TimeProvider clock) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Apply(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var nowUtc = clock.GetUtcNow().UtcDateTime;
        var userId = currentUser.UserId?.ToString();

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreated(nowUtc, userId);
                    break;
                case EntityState.Modified:
                    entry.Entity.SetModified(nowUtc, userId);
                    break;
                default:
                    break;
            }
        }
    }
}
