using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Identity;
using EssentialFrame.Time;

namespace EssentialFrame.Domain.Events;

public abstract class DomainEventBase : IDomainEvent
{
    private DomainEventBase()
    {
    }

    private DomainEventBase(IIdentity identity) : this()
    {
        TenantIdentity = identity.Tenant.Identifier;
        UserIdentity = identity.User.Identifier;
        ServiceIdentity = identity.Service.GetFullIdentifier();
        CorrelationIdentity = identity.Correlation.Identifier;
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

    protected DomainEventBase(Guid aggregateIdentifier, IIdentity identity, int expectedVersion) : this(
        aggregateIdentifier, identity)
    {
        AggregateVersion = expectedVersion;
    }

    protected DomainEventBase(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity, int expectedVersion) :
        this(aggregateIdentifier, eventIdentifier, identity)
    {
        AggregateVersion = expectedVersion;
    }

    public virtual void AdjustAggregateVersion(Guid aggregateIdentifier, int aggregateVersion)
    {
        if (AggregateIdentifier != aggregateIdentifier)
        {
            throw new DomainEventDoesNotMatchException(AggregateIdentifier, aggregateIdentifier);
        }

        AggregateVersion = aggregateVersion;
        EventTime = SystemClock.Now;
    }

    public Guid EventIdentifier { get; } = Guid.NewGuid();

    public Guid AggregateIdentifier { get; }

    public int AggregateVersion { get; private set; }

    public string ServiceIdentity { get; }

    public Guid TenantIdentity { get; }

    public Guid UserIdentity { get; }

    public Guid CorrelationIdentity { get; }

    public DateTimeOffset EventTime { get; private set; } = SystemClock.Now;
}