using EssentialFrame.Cache;
using EssentialFrame.Core.Time;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.EventsSourcing.Events.Interfaces;
using EssentialFrame.Domain.EventsSourcing.Snapshots.Interfaces;
using EssentialFrame.Domain.Snapshots;
using EssentialFrame.Serialization;

namespace EssentialFrame.Domain.EventsSourcing.Snapshots;

public class SnapshotRepository : ISnapshotRepository
{
    private readonly ICache<Guid, AggregateRoot> _cache;
    private readonly IEventRepository _eventRepository;
    private readonly IEventStore _eventStore;
    private readonly ISnapshotStore _snapshotStore;
    private readonly ISnapshotStrategy _snapshotStrategy;
    private readonly ISerializer _serializer;

    public SnapshotRepository(IEventStore eventStore,
                              IEventRepository eventRepository,
                              ISnapshotStore snapshotStore,
                              ISnapshotStrategy snapshotStrategy,
                              ISerializer serializer,
                              ICache<Guid, AggregateRoot> cache)
    {
        _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
        _snapshotStore = snapshotStore ?? throw new ArgumentNullException(nameof(snapshotStore));
        _snapshotStrategy = snapshotStrategy ?? throw new ArgumentNullException(nameof(snapshotStrategy));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
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

        var events = _eventStore
                     .Get(aggregateId, snapshotVersion)
                     .Select(e => _eventRepository.ConvertToEvent(e))
                     .Where(desc => desc.AggregateVersion > snapshotVersion);

        aggregate.Rehydrate(events);

        return aggregate;
    }

    public async Task<T> GetAsync<T>(Guid aggregateId, CancellationToken cancellationToken = default)
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
            return await _eventRepository.GetAsync<T>(aggregateId, cancellationToken);
        }

        var allEvents = await _eventStore.GetAsync(aggregateId,
                                                   snapshotVersion,
                                                   cancellationToken);

        var events = allEvents
                     .Select(e => _eventRepository.ConvertToEvent(e))
                     .Where(desc => desc.AggregateVersion > snapshotVersion);

        aggregate.Rehydrate(events);

        return aggregate;
    }

    public IEvent[] Save<T>(T aggregate,
                            int? version = null,
                            int? timeout = null)
        where T : AggregateRoot
    {
        _cache.Add(aggregate.AggregateIdentifier,
                   aggregate,
                   timeout.GetValueOrDefault(),
                   true);

        TakeSnapshot(aggregate, false);

        return _eventRepository.Save(aggregate, version);
    }

    public async Task<IEvent[]> SaveAsync<T>(T aggregate,
                                             int? version = null,
                                             int? timeout = null,
                                             CancellationToken cancellationToken = default)
        where T : AggregateRoot
    {
        _cache.Add(aggregate.AggregateIdentifier,
                   aggregate,
                   timeout.GetValueOrDefault(),
                   true);

        TakeSnapshot(aggregate, false);

        return await _eventRepository.SaveAsync(aggregate,
                                                version,
                                                cancellationToken);
    }

    public void Box<T>(T aggregate)
        where T : AggregateRoot
    {
        TakeSnapshot(aggregate, true);

        _snapshotStore.Box(aggregate.AggregateIdentifier);
        _eventStore.Box(aggregate.AggregateIdentifier);

        _cache.Remove(aggregate.AggregateIdentifier);
    }

    public async Task BoxAsync<T>(T aggregate, CancellationToken cancellationToken = default)
        where T : AggregateRoot
    {
        TakeSnapshot(aggregate, true);

        await _snapshotStore.BoxAsync(aggregate.AggregateIdentifier, cancellationToken);
        await _eventStore.BoxAsync(aggregate.AggregateIdentifier, cancellationToken);
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
            _serializer.Deserialize<AggregateState>(snapshot.AggregateState,
                                                    aggregate.CreateState().GetType());

        return aggregate;
    }

    public async Task<T> UnboxAsync<T>(Guid aggregateId, CancellationToken cancellationToken = default)
        where T : AggregateRoot
    {
        var snapshot = await _snapshotStore.UnboxAsync(aggregateId, cancellationToken);
        var aggregate = AggregateFactory<T>.CreateAggregate();

        aggregate.AggregateIdentifier = aggregateId;
        aggregate.AggregateVersion = 1;

        aggregate.State =
            _serializer.Deserialize<AggregateState>(snapshot.AggregateState,
                                                    aggregate.CreateState().GetType());

        return aggregate;
    }

    public void Ping()
    {
        var aggregates = _eventStore.GetExpired(SystemClock.Now);

        foreach (var aggregate in aggregates)
        {
            Box(Get<AggregateRoot>(aggregate));
        }
    }

    public async Task PingAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = await _eventStore.GetExpiredAsync(SystemClock.Now, cancellationToken);

        foreach (var aggregate in aggregates)
        {
            await BoxAsync(Get<AggregateRoot>(aggregate), cancellationToken);
        }
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
            _serializer.Deserialize<AggregateState>(snapshot.AggregateState,
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
                           AggregateState = _serializer.Serialize(aggregate.State)
                       };

        snapshot.AggregateVersion = aggregate.AggregateVersion + aggregate.GetUncommittedChanges().Length;

        _snapshotStore.Save(snapshot);
    }
}
