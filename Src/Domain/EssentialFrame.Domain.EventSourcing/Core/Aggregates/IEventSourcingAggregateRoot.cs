using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.Shared;
using EssentialFrame.Domain.ValueObjects.Core;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.EventSourcing.Core.Aggregates;

public interface IEventSourcingAggregateRoot<TAggregateIdentifier> : IDeletableDomainObject,
    IAggregateRoot<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    int AggregateVersion { get; }

    EventSourcingAggregateState<TAggregateIdentifier> State { get; }

    IDomainEvent<TAggregateIdentifier>[] FlushUncommittedChanges();

    void Rehydrate(IEnumerable<IDomainEvent<TAggregateIdentifier>> history);

    void RestoreState(object aggregateState, ISerializer serializer = null);
}