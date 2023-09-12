using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.Events.Core.Snapshots.Interfaces;

public interface ISnapshotStrategy<in TAggregate, TAggregateIdentifier>
    where TAggregate : IAggregateRoot<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    bool ShouldTakeSnapShot(TAggregate aggregate);
}