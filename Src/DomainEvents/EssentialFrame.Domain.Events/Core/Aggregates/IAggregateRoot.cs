namespace EssentialFrame.Domain.Events.Core.Aggregates;

public interface IAggregateRoot
{
    Guid AggregateIdentifier { get; }

    int AggregateVersion { get; }

    Guid? TenantIdentifier { get; }

    AggregateState State { get; }

    IDomainEvent[] GetUncommittedChanges();

    IDomainEvent[] FlushUncommittedChanges();

    void Rehydrate(IEnumerable<IDomainEvent> history);
}