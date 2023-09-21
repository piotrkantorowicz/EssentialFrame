using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Time;

namespace EssentialFrame.Domain.Core.Events;

public abstract class DomainEvent<TAggregateIdentifier> : IDomainEvent<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    private DomainEvent(DomainIdentity domainIdentity)
    {
        DomainEventIdentity = domainIdentity ?? throw new ArgumentNullException(nameof(domainIdentity));
    }

    protected DomainEvent(TAggregateIdentifier aggregateIdentifier, DomainIdentity domainIdentity) : this(
        domainIdentity)
    {
        AggregateIdentifier = aggregateIdentifier;
    }

    protected DomainEvent(TAggregateIdentifier aggregateIdentifier, Guid eventIdentifier, DomainIdentity domainIdentity)
        : this(aggregateIdentifier, domainIdentity)
    {
        EventIdentifier = eventIdentifier;
    }

    protected DomainEvent(TAggregateIdentifier aggregateIdentifier, DomainIdentity domainIdentity, int expectedVersion)
        : this(aggregateIdentifier, domainIdentity)
    {
        AggregateVersion = expectedVersion;
    }

    protected DomainEvent(TAggregateIdentifier aggregateIdentifier, Guid eventIdentifier, DomainIdentity domainIdentity,
        int expectedVersion) : this(aggregateIdentifier, eventIdentifier, domainIdentity)
    {
        AggregateVersion = expectedVersion;
    }

    public virtual void AdjustAggregateVersion(TAggregateIdentifier aggregateIdentifier, int aggregateVersion)
    {
        if (AggregateIdentifier != aggregateIdentifier)
        {
            throw new DomainEventDoesNotMatchException(AggregateIdentifier.ToString(), aggregateIdentifier.ToString());
        }

        AggregateVersion = aggregateVersion;
    }
    
    public Guid EventIdentifier { get; } = Guid.NewGuid();

    public TAggregateIdentifier AggregateIdentifier { get; }

    public int AggregateVersion { get; private set; }

    public DomainIdentity DomainEventIdentity { get; }

    public DateTimeOffset EventTime { get; } = SystemClock.UtcNow;
}