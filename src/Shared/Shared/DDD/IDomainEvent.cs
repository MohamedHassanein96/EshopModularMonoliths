using MediatR;

namespace Shared.DDD;

//IDomainEvent inherits from INotification to Make Any IDomainEvent Sendable through MediatR
public interface IDomainEvent : INotification 
{
    Guid EventId => Guid.NewGuid();
    public DateTime OccurredOn => DateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName!; // to manage the system from creating the correct object
}
