using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Events;

public interface IDomainEvent<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    Guid EventIdentifier { get; }

    TAggregateIdentifier AggregateIdentifier { get; }

    int AggregateVersion { get; }

    string ServiceIdentity { get; }

    Guid TenantIdentity { get; }

    Guid UserIdentity { get; }

    Guid CorrelationIdentity { get; }

    DateTimeOffset EventTime { get; }

    void AdjustAggregateVersion(TAggregateIdentifier aggregateIdentifier, int aggregateVersion);
}