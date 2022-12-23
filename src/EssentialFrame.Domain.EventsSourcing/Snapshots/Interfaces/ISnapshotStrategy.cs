using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.EventsSourcing.Snapshots.Interfaces;

public interface ISnapshotStrategy
{
    bool ShouldTakeSnapShot(AggregateRoot aggregate);
}
