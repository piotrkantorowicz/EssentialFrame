using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Snapshots;

public class SnapshotStrategy : ISnapshotStrategy
{
    private readonly int _interval;

    public SnapshotStrategy(int interval) => _interval = interval;

    public bool ShouldTakeSnapShot(AggregateRoot aggregate)
    {
        var i = aggregate.AggregateVersion;

        for (var j = 0; j < aggregate.GetUncommittedChanges().Length; j++)
        {
            if (++i % _interval == 0 &&
                i != 0)
            {
                return true;
            }
        }

        return false;
    }
}

