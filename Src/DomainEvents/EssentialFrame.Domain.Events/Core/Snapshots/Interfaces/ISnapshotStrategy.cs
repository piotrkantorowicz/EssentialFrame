using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Events.Core.Snapshots.Interfaces;

public interface ISnapshotStrategy<in TAggregate, TAggregateId> where TAggregate : AggregateRoot<TAggregateId>
    where TAggregateId : TypedGuidIdentifier
{
    bool ShouldTakeSnapShot(TAggregate aggregate);
}