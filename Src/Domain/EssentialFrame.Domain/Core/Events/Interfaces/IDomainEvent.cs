using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Events.Interfaces;

public interface IDomainEvent<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    Guid EventIdentifier { get; }

    TAggregateIdentifier AggregateIdentifier { get; }

    int AggregateVersion { get; }

    DomainIdentity DomainEventIdentity { get; }

    DateTimeOffset EventTime { get; }

    void AdjustAggregateVersion(TAggregateIdentifier aggregateIdentifier, int aggregateVersion);
}