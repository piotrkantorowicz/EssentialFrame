namespace EssentialFrame.Domain.Events;

public interface IEvent
{
    Guid EventIdentifier { get; }

    Guid AggregateIdentifier { get; }

    int AggregateVersion { get; }

    public string ServiceIdentity { get; }

    public Guid TenantIdentity { get; }

    public Guid UserIdentity { get; }

    DateTimeOffset EventTime { get; }

    void AdjustToAggregate(Guid aggregateId, int aggregateVersion);
}
