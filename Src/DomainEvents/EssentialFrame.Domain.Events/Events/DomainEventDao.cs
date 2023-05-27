using EssentialFrame.Extensions;
using EssentialFrame.Time;

namespace EssentialFrame.Domain.Events.Events;

public class DomainEventDao
{
    public DomainEventDao(IDomainEvent domainEvent)
    {
        EventIdentifier = domainEvent.EventIdentifier;
        EventClass = domainEvent.GetClassName();
        EventType = domainEvent.GetTypeFullName();
        DomainEvent = domainEvent;
        CreatedAt = SystemClock.UtcNow;
    }

    public virtual object DomainEvent { get; }

    public virtual string EventClass { get; }

    public virtual string EventType { get; }

    public virtual Guid EventIdentifier { get; }

    public virtual DateTimeOffset? CreatedAt { get; }
}