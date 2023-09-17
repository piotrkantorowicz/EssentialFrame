using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.EventSourcing.Core.Snapshots.Interfaces;

namespace EssentialFrame.Domain.EventSourcing.Core.Snapshots;

public class SnapshotStrategy<TAggregate, TAggregateIdentifier> : ISnapshotStrategy<TAggregate, TAggregateIdentifier>
    where TAggregate : class, IEventSourcingAggregateRoot<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
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