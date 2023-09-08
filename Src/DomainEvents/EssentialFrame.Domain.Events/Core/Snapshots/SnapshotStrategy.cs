using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.Events.Core.Snapshots.Interfaces;
using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Events.Core.Snapshots;

public class SnapshotStrategy<TAggregate, TAggregateId> : ISnapshotStrategy<TAggregate, TAggregateId>
    where TAggregate : AggregateRoot<TAggregateId> where TAggregateId : TypedGuidIdentifier
{
    private readonly int _interval;

    public SnapshotStrategy(int interval)
    {
        _interval = interval;
    }

    public bool ShouldTakeSnapShot(TAggregate aggregate)
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