using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.Events;

public interface IDomainEvent<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    Guid EventIdentifier { get; }

    TAggregateIdentifier AggregateIdentifier { get; }

    int AggregateVersion { get; }

    DomainEventIdentity DomainEventIdentity { get; }

    DateTimeOffset EventTime { get; }

    void AdjustAggregateVersion(TAggregateIdentifier aggregateIdentifier, int aggregateVersion);
}