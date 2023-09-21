using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.Shared;
using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.EventSourcing.Core.Aggregates;

public interface IEventSourcingAggregateRoot<TAggregateIdentifier> : IDeletableDomainObject
    where TAggregateIdentifier : TypedGuidIdentifier
{
    TAggregateIdentifier AggregateIdentifier { get; }

    TenantIdentifier TenantIdentifier { get; }

    int AggregateVersion { get; }

    IDomainEvent<TAggregateIdentifier>[] GetUncommittedChanges();

    EventSourcingAggregateState<TAggregateIdentifier> State { get; }

    IDomainEvent<TAggregateIdentifier>[] FlushUncommittedChanges();

    void Rehydrate(IEnumerable<IDomainEvent<TAggregateIdentifier>> history);

    void RestoreState(object aggregateState, ISerializer serializer = null);
}