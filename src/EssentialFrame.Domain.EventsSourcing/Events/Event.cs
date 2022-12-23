using EssentialFrame.Core.Identity;
using EssentialFrame.Core.Time;
using EssentialFrame.Domain.Events;

namespace EssentialFrame.Domain.EventsSourcing.Events;

public abstract class Event : IEvent
{
    protected Event()
    {
    }

    protected Event(IIdentity identity)
        : this()
    {
        TenantIdentity = identity.Tenant.Identifier;
        UserIdentity = identity.User.Identifier;
        ServiceIdentity = identity.Service.GetFullIdentifier();
    }

    protected Event(Guid aggregateIdentifier,
                    Guid eventIdentifier,
                    IIdentity identity)
        : this(identity)
    {
        AggregateIdentifier = aggregateIdentifier;
        EventIdentifier = eventIdentifier;
    }

    protected Event(Guid aggregateIdentifier,
                    Guid commandIdentifier,
                    IIdentity identity,
                    int expectedVersion)
        : this(aggregateIdentifier,
               commandIdentifier,
               identity) => AggregateVersion = expectedVersion;

    public virtual void AdjustToAggregate(Guid aggregateId, int aggregateVersion)
    {
        if (AggregateIdentifier == Guid.Empty)
        {
            AggregateIdentifier = aggregateId;
        }

        AggregateVersion = aggregateVersion;
        EventTime = SystemClock.Now;
    }

    public Guid EventIdentifier { get; } = Guid.NewGuid();

    public Guid AggregateIdentifier { get; private set; }

    public int AggregateVersion { get; private set; }

    public string ServiceIdentity { get; }

    public Guid TenantIdentity { get; }

    public Guid UserIdentity { get; }

    public DateTimeOffset EventTime { get; private set; } = SystemClock.Now;
}
