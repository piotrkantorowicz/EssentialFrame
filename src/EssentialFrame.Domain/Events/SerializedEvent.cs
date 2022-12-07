using EssentialFrame.Domain.Core;

namespace EssentialFrame.Domain.Events;

public class SerializedEvent : IEvent
{
    public SerializedEvent() => EventTime = DateTimeOffset.UtcNow;

    public string EventClass { get; set; }

    public string EventType { get; set; }

    public string EventData { get; set; }

    public Guid AggregateIdentifier { get; set; }

    public int AggregateVersion { get; set; }

    public Guid IdentityTenant { get; set; }

    public Guid IdentityUser { get; set; }

    public DateTimeOffset EventTime { get; set; }
}
