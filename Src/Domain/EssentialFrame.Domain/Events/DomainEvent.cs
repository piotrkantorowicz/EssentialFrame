using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.ValueObjects.Core;
using EssentialFrame.Identity;
using EssentialFrame.Time;

namespace EssentialFrame.Domain.Events;

public abstract class DomainEvent<TAggregateIdentifier> : IDomainEvent<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    private DomainEvent(IIdentityContext identityContext)
    {
        if (identityContext == null)
        {
            throw new ArgumentNullException(nameof(identityContext));
        }

        DomainEventIdentity = new DomainEventIdentity(identityContext.Tenant.Identifier,
            identityContext.User.Identifier, identityContext.Correlation.Identifier,
            identityContext.Service.GetFullIdentifier());
    }

    protected DomainEvent(TAggregateIdentifier aggregateIdentifier, IIdentityContext identityContext) : this(
        identityContext)
    {
        AggregateIdentifier = aggregateIdentifier;
    }

    protected DomainEvent(TAggregateIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext) : this(aggregateIdentifier, identityContext)
    {
        EventIdentifier = eventIdentifier;
    }

    protected DomainEvent(TAggregateIdentifier aggregateIdentifier, IIdentityContext identityContext,
        int expectedVersion) : this(aggregateIdentifier, identityContext)
    {
        AggregateVersion = expectedVersion;
    }

    protected DomainEvent(TAggregateIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext,
        int expectedVersion) : this(aggregateIdentifier, eventIdentifier, identityContext)
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

    public DomainEventIdentity DomainEventIdentity { get; }

    public DateTimeOffset EventTime { get; } = SystemClock.UtcNow;
}