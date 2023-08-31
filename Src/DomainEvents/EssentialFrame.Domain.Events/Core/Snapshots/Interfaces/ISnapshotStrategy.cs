using EssentialFrame.Domain.Events.Core.Aggregates;

namespace EssentialFrame.Domain.Events.Core.Snapshots.Interfaces;

public interface ISnapshotStrategy
{
    bool ShouldTakeSnapShot(AggregateRoot aggregate);
}