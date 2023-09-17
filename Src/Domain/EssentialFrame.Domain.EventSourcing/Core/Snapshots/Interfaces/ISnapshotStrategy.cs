using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;

namespace EssentialFrame.Domain.EventSourcing.Core.Snapshots.Interfaces;

public interface ISnapshotStrategy<in TAggregate, TAggregateIdentifier>
    where TAggregate : class, IEventSourcingAggregateRoot<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    bool ShouldTakeSnapShot(TAggregate aggregate);
}