using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.EventSourcing.Core.Snapshots.Interfaces;

public interface ISnapshotStrategy<in TAggregate, TAggregateIdentifier>
    where TAggregate : IEventSourcingAggregateRoot<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    bool ShouldTakeSnapShot(TAggregate aggregate);
}