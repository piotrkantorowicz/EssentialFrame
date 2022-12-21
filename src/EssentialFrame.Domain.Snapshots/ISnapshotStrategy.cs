using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.Snapshots;

public interface ISnapshotStrategy
{
    bool ShouldTakeSnapShot(AggregateRoot aggregate);
}

