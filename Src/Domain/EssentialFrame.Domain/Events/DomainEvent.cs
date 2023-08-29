using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Identity;
using EssentialFrame.Time;

namespace EssentialFrame.Domain.Events;

public abstract class DomainEvent : IDomainEvent
{
    private DomainEvent(IIdentityContext identityContext) 
    {
        TenantIdentity = identityContext?.Tenant?.Identifier ?? Guid.Empty;
        UserIdentity = identityContext?.User?.Identifier ?? Guid.Empty;
        CorrelationIdentity = identityContext?.Correlation?.Identifier ?? Guid.Empty;
        ServiceIdentity = identityContext?.Service?.GetFullIdentifier();
    }

    protected DomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext) : this(identityContext)
    {
        AggregateIdentifier = aggregateIdentifier;
    }

    protected DomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentityContext identityContext) : this(
        aggregateIdentifier, identityContext)
    {
        EventIdentifier = eventIdentifier;
    }

    protected DomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext, int expectedVersion) : this(
        aggregateIdentifier, identityContext)
    {
        AggregateVersion = expectedVersion;
    }

    protected DomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentityContext identityContext,
        int expectedVersion) : this(aggregateIdentifier, eventIdentifier, identityContext)
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
    }

    public Guid EventIdentifier { get; } = Guid.NewGuid();

    public Guid AggregateIdentifier { get; }

    public int AggregateVersion { get; private set; }

    public string ServiceIdentity { get; }

    public Guid TenantIdentity { get; }

    public Guid UserIdentity { get; }

    public Guid CorrelationIdentity { get; }

    public DateTimeOffset EventTime { get; private set; } = SystemClock.UtcNow;
}