using EssentialFrame.Core.Extensions;
using EssentialFrame.Core.Time;
using EssentialFrame.Domain.Events;

namespace EssentialFrame.Domain.EventsSourcing.Events;

public class EventData
{
    public EventData(IEvent @event)
    {
        EventIdentifier = @event.EventIdentifier;

        EventClass = @event.GetClassName();
        EventType = @event.GetTypeFullName();
        Event = @event;
        CreatedAt = SystemClock.Now;
    }

    public virtual object Event { get; }

    public virtual string EventClass { get; }

    public virtual string EventType { get; }

    public virtual Guid EventIdentifier { get; }

    public virtual DateTimeOffset? CreatedAt { get; }
}
