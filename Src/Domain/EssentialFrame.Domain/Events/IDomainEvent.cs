namespace EssentialFrame.Domain.Events;

public interface IDomainEvent
{
    Guid EventIdentifier { get; }

    Guid AggregateIdentifier { get; }

    int AggregateVersion { get; }

    public string ServiceIdentity { get; }

    public Guid TenantIdentity { get; }

    public Guid UserIdentity { get; }

    DateTimeOffset EventTime { get; }

    void AdjustAggregateVersion(Guid aggregateIdentifier, int aggregateVersion);
}