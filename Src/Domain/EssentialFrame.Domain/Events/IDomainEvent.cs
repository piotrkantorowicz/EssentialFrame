namespace EssentialFrame.Domain.Events;

public interface IDomainEvent
{
    Guid EventIdentifier { get; }

    Guid AggregateIdentifier { get; }

    int AggregateVersion { get; }

    string ServiceIdentity { get; }

    Guid TenantIdentity { get; }

    Guid UserIdentity { get; }

    Guid CorrelationIdentity { get; }

    DateTimeOffset EventTime { get; }

    void AdjustAggregateVersion(Guid aggregateIdentifier, int aggregateVersion);
}