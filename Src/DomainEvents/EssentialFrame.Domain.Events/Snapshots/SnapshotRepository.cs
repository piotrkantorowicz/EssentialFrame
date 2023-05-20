using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Events;
using EssentialFrame.Domain.Events.Events.Interfaces;
using EssentialFrame.Domain.Events.Snapshots.Interfaces;
using EssentialFrame.Domain.Factories;
using EssentialFrame.Domain.Snapshots;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Events.Snapshots;

public class SnapshotRepository : ISnapshotRepository
{
    private readonly ICache<Guid, AggregateRoot> _cache;
    private readonly IDomainEventsRepository _domainEventsRepository;
    private readonly IDomainEventsStore _domainEventsStore;
    private readonly ISerializer _serializer;
    private readonly ISnapshotStore _snapshotStore;
    private readonly ISnapshotStrategy _snapshotStrategy;

    public SnapshotRepository(IDomainEventsStore domainEventsStore, IDomainEventsRepository domainEventsRepository,
        ISnapshotStore snapshotStore, ISnapshotStrategy snapshotStrategy, ISerializer serializer,
        ICache<Guid, AggregateRoot> cache)
    {
        _domainEventsStore = domainEventsStore ?? throw new ArgumentNullException(nameof(domainEventsStore));
        _domainEventsRepository =
            domainEventsRepository ?? throw new ArgumentNullException(nameof(domainEventsRepository));
        _snapshotStore = snapshotStore ?? throw new ArgumentNullException(nameof(snapshotStore));
        _snapshotStrategy = snapshotStrategy ?? throw new ArgumentNullException(nameof(snapshotStrategy));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public T Get<T>(Guid aggregateId) where T : AggregateRoot
    {
        AggregateRoot snapshot = _cache.Get(aggregateId);

        if (snapshot != null)
        {
            return (T)snapshot;
        }

        T aggregate = GenericAggregateFactory<T>.CreateAggregate(aggregateId, 0);
        int snapshotVersion = RestoreAggregateFromSnapshot(aggregateId, aggregate);

        if (snapshotVersion == -1)
        {
            return _domainEventsRepository.Get<T>(aggregateId);
        }

        IEnumerable<IDomainEvent> events = _domainEventsStore.Get(aggregateId, snapshotVersion)
            .Select(e => _domainEventsRepository.ConvertToEvent(e))
            .Where(desc => desc.AggregateVersion > snapshotVersion);

        aggregate.Rehydrate(events);

        return aggregate;
    }

    public async Task<T> GetAsync<T>(Guid aggregateId, CancellationToken cancellationToken = default)
        where T : AggregateRoot
    {
        AggregateRoot snapshot = _cache.Get(aggregateId);

        if (snapshot != null)
        {
            return (T)snapshot;
        }

        T aggregate = GenericAggregateFactory<T>.CreateAggregate(aggregateId, 0);
        int snapshotVersion = RestoreAggregateFromSnapshot(aggregateId, aggregate);

        if (snapshotVersion == -1)
        {
            return await _domainEventsRepository.GetAsync<T>(aggregateId, cancellationToken);
        }

        IReadOnlyCollection<DomainEventDao> allEvents = await _domainEventsStore.GetAsync(aggregateId,
            snapshotVersion, cancellationToken);

        IEnumerable<IDomainEvent> events = allEvents.Select(e => _domainEventsRepository.ConvertToEvent(e))
            .Where(desc => desc.AggregateVersion > snapshotVersion);

        aggregate.Rehydrate(events);

        return aggregate;
    }

    public IDomainEvent[] Save<T>(T aggregate, int? version = null, int? timeout = null) where T : AggregateRoot
    {
        _cache.Add(aggregate.AggregateIdentifier, aggregate, timeout.GetValueOrDefault(), true);

        TakeSnapshot(aggregate, false);

        return _domainEventsRepository.Save(aggregate, version);
    }

    public async Task<IDomainEvent[]> SaveAsync<T>(T aggregate, int? version = null, int? timeout = null,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        _cache.Add(aggregate.AggregateIdentifier, aggregate, timeout.GetValueOrDefault(), true);

        TakeSnapshot(aggregate, false);

        return await _domainEventsRepository.SaveAsync(aggregate, version, cancellationToken);
    }

    public void Box<T>(T aggregate) where T : AggregateRoot
    {
        TakeSnapshot(aggregate, true);

        _snapshotStore.Box(aggregate.AggregateIdentifier);
        _domainEventsStore.Box(aggregate.AggregateIdentifier);

        _cache.Remove(aggregate.AggregateIdentifier);
    }

    public async Task BoxAsync<T>(T aggregate, CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        TakeSnapshot(aggregate, true);

        await _snapshotStore.BoxAsync(aggregate.AggregateIdentifier, cancellationToken);
        await _domainEventsStore.BoxAsync(aggregate.AggregateIdentifier, cancellationToken);

        _cache.Remove(aggregate.AggregateIdentifier);
    }

    public T Unbox<T>(Guid aggregateId) where T : AggregateRoot
    {
        Snapshot snapshot = _snapshotStore.Unbox(aggregateId);
        T aggregate = GenericAggregateFactory<T>.CreateAggregate(aggregateId, 1);

        aggregate.RestoreState(snapshot.AggregateState, _serializer);

        return aggregate;
    }

    public async Task<T> UnboxAsync<T>(Guid aggregateId, CancellationToken cancellationToken = default)
        where T : AggregateRoot
    {
        Snapshot snapshot = await _snapshotStore.UnboxAsync(aggregateId, cancellationToken);
        T aggregate = GenericAggregateFactory<T>.CreateAggregate(aggregateId, 1);

        aggregate.RestoreState(snapshot.AggregateState, _serializer);

        return aggregate;
    }

    public void Ping()
    {
        IEnumerable<Guid> aggregates = _domainEventsStore.GetTerminated();

        foreach (Guid aggregate in aggregates)
        {
            Box(Get<AggregateRoot>(aggregate));
        }
    }

    public async Task PingAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Guid> aggregates = await _domainEventsStore.GetTerminatedAsync(cancellationToken);

        foreach (Guid aggregate in aggregates)
        {
            await BoxAsync(Get<AggregateRoot>(aggregate), cancellationToken);
        }
    }

    private int RestoreAggregateFromSnapshot<T>(Guid id, T aggregate) where T : AggregateRoot
    {
        Snapshot snapshot = _snapshotStore.Get(id);

        if (snapshot == null)
        {
            return -1;
        }

        aggregate.RestoreState(snapshot.AggregateState, _serializer);

        return snapshot.AggregateVersion;
    }

    private void TakeSnapshot(AggregateRoot aggregate, bool force)
    {
        if (!force && !_snapshotStrategy.ShouldTakeSnapShot(aggregate))
        {
            return;
        }

        int aggregateVersion = aggregate.AggregateVersion + aggregate.GetUncommittedChanges().Length;
        string aggregateState = _serializer.Serialize(aggregate.State);
        Snapshot snapshot = new(aggregate.AggregateIdentifier, aggregateVersion, aggregateState);

        _snapshotStore.Save(snapshot);
    }
}