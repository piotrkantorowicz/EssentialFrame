using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.Events.Snapshots.Interfaces;

public interface ISnapshotStrategy
{
    bool ShouldTakeSnapShot(AggregateRoot aggregate);
}