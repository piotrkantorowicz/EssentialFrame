using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Snapshots;

public interface ISnapshotStrategy
{
    bool ShouldTakeSnapShot(AggregateRoot aggregate);
}
