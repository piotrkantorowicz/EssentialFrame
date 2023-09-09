using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Events.Core.Aggregates;

public interface IAggregateRoot<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    TAggregateIdentifier AggregateIdentifier { get; }

    int AggregateVersion { get; }

    Guid? TenantIdentifier { get; }

    AggregateState<TAggregateIdentifier> State { get; }

    IDomainEvent<TAggregateIdentifier>[] GetUncommittedChanges();

    IDomainEvent<TAggregateIdentifier>[] FlushUncommittedChanges();

    void Rehydrate(IEnumerable<IDomainEvent<TAggregateIdentifier>> history);
}