using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Core.Snapshots.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Models;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Services.Interfaces;
using EssentialFrame.Domain.Factories;
using EssentialFrame.Domain.Snapshots;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Events.Persistence.Snapshots.Services;

public class SnapshotRepository : ISnapshotRepository
{
    private readonly ICache<Guid, AggregateRoot> _cache;
    private readonly IAggregateRepository _aggregateRepository;
    private readonly IAggregateStore _aggregateStore;
    private readonly IDomainEventMapper _domainEventMapper;
    private readonly IIdentityService _identityService;
    private readonly ISerializer _serializer;
    private readonly ISnapshotMapper _snapshotMapper;
    private readonly ISnapshotStore _snapshotStore;
    private readonly ISnapshotStrategy _snapshotStrategy;

    public SnapshotRepository(IAggregateStore aggregateStore, IAggregateRepository aggregateRepository,
        ISnapshotStore snapshotStore, ISnapshotStrategy snapshotStrategy, ISerializer serializer,
        ICache<Guid, AggregateRoot> cache, ISnapshotMapper snapshotMapper, IDomainEventMapper domainEventMapper,
        IIdentityService identityService)
    {
        _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore));
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
        _snapshotStore = snapshotStore ?? throw new ArgumentNullException(nameof(snapshotStore));
        _snapshotStrategy = snapshotStrategy ?? throw new ArgumentNullException(nameof(snapshotStrategy));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _snapshotMapper = snapshotMapper ?? throw new ArgumentNullException(nameof(snapshotMapper));
        _domainEventMapper = domainEventMapper ?? throw new ArgumentNullException(nameof(domainEventMapper));
        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
    }

    public T Get<T>(Guid aggregateId) where T : AggregateRoot
    {
        AggregateRoot snapshot = _cache.Get(aggregateId);

        if (snapshot != null)
        {
            return (T)snapshot;
        }

        T aggregate = GenericAggregateFactory<T>.CreateAggregate(aggregateId, 0, _identityService);
        int snapshotVersion = RestoreAggregateFromSnapshot(aggregateId, aggregate);

        if (snapshotVersion == -1)
        {
            return _aggregateRepository.Get<T>(aggregateId);
        }

        IEnumerable<IDomainEvent> events = _aggregateStore.Get(aggregateId, snapshotVersion)
            .Select(e => _domainEventMapper.Map(e)).Where(desc => desc.AggregateVersion > snapshotVersion);

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

        T aggregate = GenericAggregateFactory<T>.CreateAggregate(aggregateId, 0, _identityService);
        int snapshotVersion = RestoreAggregateFromSnapshot(aggregateId, aggregate);

        if (snapshotVersion == -1)
        {
            return await _aggregateRepository.GetAsync<T>(aggregateId, cancellationToken);
        }

        IReadOnlyCollection<DomainEventDataModel> allEvents = await _aggregateStore.GetAsync(aggregateId,
            snapshotVersion, cancellationToken);

        IEnumerable<IDomainEvent> events = allEvents.Select(e => _domainEventMapper.Map(e))
            .Where(desc => desc.AggregateVersion > snapshotVersion);

        aggregate.Rehydrate(events);

        return aggregate;
    }

    public IDomainEvent[] Save<T>(T aggregate, int? version = null, int? timeout = null) where T : AggregateRoot
    {
        _cache.Add(aggregate.AggregateIdentifier, aggregate, timeout.GetValueOrDefault(), true);

        TakeSnapshot(aggregate, false);

        return _aggregateRepository.Save(aggregate, version);
    }

    public async Task<IDomainEvent[]> SaveAsync<T>(T aggregate, int? version = null, int? timeout = null,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        _cache.Add(aggregate.AggregateIdentifier, aggregate, timeout.GetValueOrDefault(), true);

        TakeSnapshot(aggregate, false);

        return await _aggregateRepository.SaveAsync(aggregate, version, cancellationToken);
    }

    public void Box<T>(T aggregate) where T : AggregateRoot
    {
        TakeSnapshot(aggregate, true);

        _snapshotStore.Box(aggregate.AggregateIdentifier);
        _aggregateStore.Box(aggregate.AggregateIdentifier);

        _cache.Remove(aggregate.AggregateIdentifier);
    }

    public async Task BoxAsync<T>(T aggregate, CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        TakeSnapshot(aggregate, true);

        await _snapshotStore.BoxAsync(aggregate.AggregateIdentifier, cancellationToken);
        await _aggregateStore.BoxAsync(aggregate.AggregateIdentifier, cancellationToken);

        _cache.Remove(aggregate.AggregateIdentifier);
    }

    public T Unbox<T>(Guid aggregateId) where T : AggregateRoot
    {
        SnapshotDataModel snapshotDataModel = _snapshotStore.Unbox(aggregateId);
        Snapshot snapshot = _snapshotMapper.Map(snapshotDataModel);
        T aggregate = GenericAggregateFactory<T>.CreateAggregate(aggregateId, 1, _identityService);

        aggregate.RestoreState(snapshot.AggregateState, _serializer);

        return aggregate;
    }

    public async Task<T> UnboxAsync<T>(Guid aggregateId, CancellationToken cancellationToken = default)
        where T : AggregateRoot
    {
        SnapshotDataModel snapshotDataModel = await _snapshotStore.UnboxAsync(aggregateId, cancellationToken);
        Snapshot snapshot = _snapshotMapper.Map(snapshotDataModel);
        T aggregate = GenericAggregateFactory<T>.CreateAggregate(aggregateId, 1, _identityService);

        aggregate.RestoreState(snapshot.AggregateState, _serializer);

        return aggregate;
    }

    public void Ping()
    {
        IEnumerable<Guid> aggregates = _aggregateStore.GetDeleted();

        foreach (Guid aggregate in aggregates)
        {
            Box(Get<AggregateRoot>(aggregate));
        }
    }

    public async Task PingAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Guid> aggregates = await _aggregateStore.GetDeletedAsync(cancellationToken);

        foreach (Guid aggregate in aggregates)
        {
            await BoxAsync(Get<AggregateRoot>(aggregate), cancellationToken);
        }
    }

    private int RestoreAggregateFromSnapshot<T>(Guid id, T aggregate) where T : AggregateRoot
    {
        SnapshotDataModel snapshotDataModel = _snapshotStore.Get(id);
        Snapshot snapshot = _snapshotMapper.Map(snapshotDataModel);

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
        SnapshotDataModel snapshotDataModel = _snapshotMapper.Map(snapshot);

        _snapshotStore.Save(snapshotDataModel);
    }
}