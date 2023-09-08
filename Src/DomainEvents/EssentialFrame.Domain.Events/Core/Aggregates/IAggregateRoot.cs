using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Events.Core.Aggregates;

public interface IAggregateRoot<out T, TType> where T : TypedIdentifierBase<TType>
{
    T AggregateIdentifier { get; }

    int AggregateVersion { get; }

    Guid? TenantIdentifier { get; }

    AggregateState State { get; }

    IDomainEvent[] GetUncommittedChanges();

    IDomainEvent[] FlushUncommittedChanges();

    void Rehydrate(IEnumerable<IDomainEvent> history);
}