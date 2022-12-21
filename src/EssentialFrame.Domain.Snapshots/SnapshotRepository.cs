using EssentialFrame.Cache;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Core;

namespace EssentialFrame.Domain.Snapshots;

public class SnapshotRepository : IEventRepository
{
    private readonly GuidCache<AggregateRoot> _cache = new();
    private readonly IEventRepository _eventRepository;
    private readonly IEventStore _eventStore;
    private readonly ISnapshotStore _snapshotStore;
    private readonly ISnapshotStrategy _snapshotStrategy;

    public SnapshotRepository(IEventStore eventStore,
                              IEventRepository eventRepository,
                              ISnapshotStore snapshotStore,
                              ISnapshotStrategy snapshotStrategy)
    {
        _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
        _snapshotStore = snapshotStore ?? throw new ArgumentNullException(nameof(snapshotStore));
        _snapshotStrategy = snapshotStrategy ?? throw new ArgumentNullException(nameof(snapshotStrategy));
    }

    public IEvent[] Save<T>(T aggregate, int? version = null)
        where T : AggregateRoot
    {
        _cache.Add(aggregate.AggregateIdentifier,
                   aggregate,
                   5 * 60,
                   true);

        TakeSnapshot(aggregate, false);

        return _eventRepository.Save(aggregate, version);
    }

    public T Get<T>(Guid aggregateId)
        where T : AggregateRoot
    {
        var snapshot = _cache.Get(aggregateId);

        if (snapshot != null)
        {
            return (T)snapshot;
        }

        var aggregate = AggregateFactory<T>.CreateAggregate();
        var snapshotVersion = RestoreAggregateFromSnapshot(aggregateId, aggregate);

        if (snapshotVersion == -1)
        {
            return _eventRepository.Get<T>(aggregateId);
        }

        var events = _eventStore.Get(aggregateId, snapshotVersion)
                                .Where(desc => desc.AggregateVersion > snapshotVersion);

        aggregate.Rehydrate(events);

        return aggregate;
    }

    public void Box<T>(T aggregate)
        where T : AggregateRoot
    {
        TakeSnapshot(aggregate, true);

        _snapshotStore.Box(aggregate.AggregateIdentifier);
        _eventStore.Box(aggregate.AggregateIdentifier);

        _cache.Remove(aggregate.AggregateIdentifier);
    }

    public T Unbox<T>(Guid aggregateId)
        where T : AggregateRoot
    {
        var snapshot = _snapshotStore.Unbox(aggregateId);
        var aggregate = AggregateFactory<T>.CreateAggregate();

        aggregate.AggregateIdentifier = aggregateId;
        aggregate.AggregateVersion = 1;

        aggregate.State =
            _eventStore.Serializer.Deserialize<AggregateState>(snapshot.AggregateState,
                                                               aggregate.CreateState().GetType());

        return aggregate;
    }

    private int RestoreAggregateFromSnapshot<T>(Guid id, T aggregate)
        where T : AggregateRoot
    {
        var snapshot = _snapshotStore.Get(id);

        if (snapshot == null)
        {
            return -1;
        }

        aggregate.AggregateIdentifier = snapshot.AggregateIdentifier;
        aggregate.AggregateVersion = snapshot.AggregateVersion;

        aggregate.State =
            _eventStore.Serializer.Deserialize<AggregateState>(snapshot.AggregateState,
                                                               aggregate.CreateState().GetType());

        return snapshot.AggregateVersion;
    }

    private void TakeSnapshot(AggregateRoot aggregate, bool force)
    {
        if (!force &&
            !_snapshotStrategy.ShouldTakeSnapShot(aggregate))
        {
            return;
        }

        var snapshot = new Snapshot
                       {
                           AggregateIdentifier = aggregate.AggregateIdentifier,
                           AggregateVersion = aggregate.AggregateVersion,
                           AggregateState = _eventStore.Serializer.Serialize(aggregate.State)
                       };

        snapshot.AggregateVersion = aggregate.AggregateVersion + aggregate.GetUncommittedChanges().Length;

        _snapshotStore.Save(snapshot);
    }

    public void Ping()
    {
        var aggregates = _eventStore.GetExpired(DateTimeOffset.UtcNow);

        foreach (var aggregate in aggregates)
        {
            Box(Get<AggregateRoot>(aggregate));
        }
    }
}

