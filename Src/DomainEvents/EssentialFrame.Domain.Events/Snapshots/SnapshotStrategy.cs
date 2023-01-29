using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Snapshots.Interfaces;

namespace EssentialFrame.Domain.Events.Snapshots;

public class SnapshotStrategy : ISnapshotStrategy
{
    private readonly int _interval;

    public SnapshotStrategy(int interval)
    {
        _interval = interval;
    }

    public bool ShouldTakeSnapShot(AggregateRoot aggregate)
    {
        int i = aggregate.AggregateVersion;

        for (int j = 0; j < aggregate.GetUncommittedChanges().Length; j++)
        {
            if (++i % _interval == 0 && i != 0)
            {
                return true;
            }
        }

        return false;
    }
}