using EssentialFrame.Domain.Shared;
using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.Domain.ValueObjects.Core;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Events.Core.Aggregates;

public interface IAggregateRoot<TAggregateIdentifier> : IDeletableDomainObject
    where TAggregateIdentifier : TypedGuidIdentifier
{
    TAggregateIdentifier AggregateIdentifier { get; }

    int AggregateVersion { get; }

    TenantIdentifier TenantIdentifier { get; }

    AggregateState<TAggregateIdentifier> State { get; }

    IDomainEvent<TAggregateIdentifier>[] GetUncommittedChanges();

    IDomainEvent<TAggregateIdentifier>[] FlushUncommittedChanges();

    void Rehydrate(IEnumerable<IDomainEvent<TAggregateIdentifier>> history);

    void RestoreState(object aggregateState, ISerializer serializer = null);
}