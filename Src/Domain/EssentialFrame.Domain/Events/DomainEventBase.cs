using EssentialFrame.Identity;
using EssentialFrame.Time;

namespace EssentialFrame.Domain.Events;

public abstract class DomainEventBase : IDomainEvent
{
    protected DomainEventBase()
    {
    }

    protected DomainEventBase(IIdentity identity) : this()
    {
        TenantIdentity = identity.Tenant.Identifier;
        UserIdentity = identity.User.Identifier;
        ServiceIdentity = identity.Service.GetFullIdentifier();
    }

    protected DomainEventBase(Guid aggregateIdentifier, IIdentity identity) : this(identity)
    {
        AggregateIdentifier = aggregateIdentifier;
    }

    protected DomainEventBase(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity) : this(identity)
    {
        AggregateIdentifier = aggregateIdentifier;
        EventIdentifier = eventIdentifier;
    }

    protected DomainEventBase(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity, int expectedVersion) :
        this(aggregateIdentifier, eventIdentifier, identity)
    {
        AggregateVersion = expectedVersion;
    }

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