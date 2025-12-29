using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Shared.DDD;

namespace Shared.Data.Interceptors;

public class DispatchDomainEventInterceptor(IMediator mediator , ILogger<DispatchDomainEventInterceptor> logger) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        DispatchDomainEvents(eventData.Context).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {


        await DispatchDomainEvents(eventData.Context);
        return  await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEvents(DbContext? context)
    {

        logger.LogInformation(
       "Interceptor {Name}, DetectChangesCount = {Count}",
       nameof(DispatchDomainEventInterceptor),
       context.ChangeTracker.Entries().Count());


        if (context == null)
            return;

        var aggregates = context.ChangeTracker
            .Entries<IAggregate>()
            .Where(a => a.Entity.DomainEvents.Any())
            .Select(a => a.Entity);


        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        aggregates.ToList().ForEach(a => a.ClearDomainEvents()); // Review

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }

    }
}
