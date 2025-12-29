using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Shared.DDD;

namespace Shared.Data.Interceptors;

public class AuditableEntityInterceptor(ILogger<AuditableEntityInterceptor> logger) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {

        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {

        // to be deleted
        logger.LogInformation(
       "Interceptor {Name}, DetectChangesCount = {Count}",
       nameof(AuditableEntityInterceptor),
       context.ChangeTracker.Entries().Count());


        if (context == null)
            return;

        foreach (var entry in context.ChangeTracker.Entries<IEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = "Mohamed";
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                entry.Entity.LastModifiedBy = "Mohamed";
                entry.Entity.LastModified = DateTime.UtcNow;
            }
        }
    }
}

public static class Extensions
{
   
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>

        entry.References.Any(r =>
        r.TargetEntry != null &&
        r.TargetEntry.Metadata.IsOwned() &&
       (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));

}