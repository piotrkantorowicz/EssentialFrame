using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.Events.Core.Factories;
using EssentialFrame.Domain.Events.Core.Snapshots;
using EssentialFrame.Domain.Events.Core.Snapshots.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Models;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Services.Interfaces;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Events.Persistence.Snapshots.Services;

public class SnapshotRepository : ISnapshotRepository
{
    private readonly IAggregateRepository _aggregateRepository;
    private readonly IAggregateStore _aggregateStore;
    private readonly ICache<Guid, AggregateRoot> _snapshotsCache;
    private readonly IDomainEventMapper _domainEventMapper;
    private readonly ISerializer _serializer;
    private readonly ISnapshotMapper _snapshotMapper;
    private readonly ISnapshotStore _snapshotStore;
    private readonly ISnapshotStrategy _snapshotStrategy;

    public SnapshotRepository(IAggregateStore aggregateStore, IAggregateRepository aggregateRepository,
        ISnapshotStore snapshotStore, ISnapshotStrategy snapshotStrategy, ISerializer serializer,
        ICache<Guid, AggregateRoot> snapshotsCache, ISnapshotMapper snapshotMapper,
        IDomainEventMapper domainEventMapper)
    {
        _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore));
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
        _snapshotStore = snapshotStore ?? throw new ArgumentNullException(nameof(snapshotStore));
        _snapshotStrategy = snapshotStrategy ?? throw new ArgumentNullException(nameof(snapshotStrategy));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _snapshotsCache = snapshotsCache ?? throw new ArgumentNullException(nameof(snapshotsCache));
        _snapshotMapper = snapshotMapper ?? throw new ArgumentNullException(nameof(snapshotMapper));
        _domainEventMapper = domainEventMapper ?? throw new ArgumentNullException(nameof(domainEventMapper));
    }

    public T Get<T>(Guid aggregateId, IIdentityContext identityContext) where T : AggregateRoot
    {
        AggregateRoot snapshot = _snapshotsCache.Get(aggregateId);

        if (snapshot != null)
        {
            return (T)snapshot;
        }

        T aggregate = GenericAggregateFactory<T>.CreateAggregate(aggregateId, identityContext);
        int snapshotVersion = RestoreAggregateFromSnapshot(aggregateId, aggregate);

        if (snapshotVersion == -1)
        {
            return _aggregateRepository.Get<T>(aggregateId, identityContext);
        }

        IReadOnlyCollection<DomainEventDataModel> allEvents = _aggregateStore.Get(aggregateId, snapshotVersion);

        IEnumerable<IDomainEvent> events =
            _domainEventMapper.Map(allEvents.Where(desc => desc.AggregateVersion <= snapshotVersion));

        aggregate.Rehydrate(events);

        return aggregate;
    }

    public async Task<T> GetAsync<T>(Guid aggregateId, IIdentityContext identityContext,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        AggregateRoot snapshot = _snapshotsCache.Get(aggregateId);

        if (snapshot != null)
        {
            return (T)snapshot;
        }

        T aggregate = GenericAggregateFactory<T>.CreateAggregate(aggregateId, identityContext);
        int snapshotVersion = await RestoreAggregateFromSnapshotAsync(aggregateId, aggregate, cancellationToken);

        if (snapshotVersion == -1)
        {
            return await _aggregateRepository.GetAsync<T>(aggregateId, identityContext, cancellationToken);
        }

        IReadOnlyCollection<DomainEventDataModel> allEvents = await _aggregateStore.GetAsync(aggregateId,
            snapshotVersion, cancellationToken);

        IEnumerable<IDomainEvent> events =
            _domainEventMapper.Map(allEvents.Where(desc => desc.AggregateVersion <= snapshotVersion));

        aggregate.Rehydrate(events);

        return aggregate;
    }

    public IDomainEvent[] Save<T>(T aggregate, int? version = null, int? timeout = null) where T : AggregateRoot
    {
        if (timeout is > 0)
        {
            _snapshotsCache.Add(aggregate.AggregateIdentifier, aggregate, timeout.GetValueOrDefault(), true);
        }

        TakeSnapshot(aggregate, false, false);

        return _aggregateRepository.Save(aggregate, version);
    }

    public async Task<IDomainEvent[]> SaveAsync<T>(T aggregate, int? version = null, int? timeout = null,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        if (timeout is > 0)
        {
            _snapshotsCache.Add(aggregate.AggregateIdentifier, aggregate, timeout.GetValueOrDefault(), true);
        }

        TakeSnapshot(aggregate, false, false);

        return await _aggregateRepository.SaveAsync(aggregate, version, cancellationToken);
    }

    public void Box<T>(T aggregate, bool useSerializer = false) where T : AggregateRoot
    {
        SnapshotDataModel snapshotDataModel = TakeSnapshot(aggregate, useSerializer, true);

        _snapshotStore.Save(snapshotDataModel);
        _snapshotStore.Box(aggregate.AggregateIdentifier);
        _aggregateStore.Box(aggregate.AggregateIdentifier);

        _snapshotsCache.Remove(aggregate.AggregateIdentifier);
    }

    public async Task BoxAsync<T>(T aggregate, bool useSerializer = false,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        SnapshotDataModel snapshotDataModel = TakeSnapshot(aggregate, useSerializer, true);

        await _snapshotStore.SaveAsync(snapshotDataModel, cancellationToken);
        await _snapshotStore.BoxAsync(aggregate.AggregateIdentifier, cancellationToken);
        await _aggregateStore.BoxAsync(aggregate.AggregateIdentifier, cancellationToken);

        _snapshotsCache.Remove(aggregate.AggregateIdentifier);
    }

    public T Unbox<T>(Guid aggregateId, IIdentityContext identityContext, bool useSerializer = false)
        where T : AggregateRoot
    {
        SnapshotDataModel snapshotDataModel = _snapshotStore.Unbox(aggregateId);
        Snapshot snapshot = _snapshotMapper.Map(snapshotDataModel);
        T aggregate =
            GenericAggregateFactory<T>.CreateAggregate(aggregateId, snapshot.AggregateVersion, identityContext);

        aggregate.RestoreState(snapshot.AggregateState, useSerializer ? _serializer : null);

        return aggregate;
    }

    public async Task<T> UnboxAsync<T>(Guid aggregateId, IIdentityContext identityContext, bool useSerializer = false,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        SnapshotDataModel snapshotDataModel = await _snapshotStore.UnboxAsync(aggregateId, cancellationToken);
        Snapshot snapshot = _snapshotMapper.Map(snapshotDataModel);
        T aggregate =
            GenericAggregateFactory<T>.CreateAggregate(aggregateId, snapshot.AggregateVersion, identityContext);

        aggregate.RestoreState(snapshot.AggregateState, useSerializer ? _serializer : null);

        return aggregate;
    }

    private int RestoreAggregateFromSnapshot<T>(Guid id, T aggregate) where T : AggregateRoot
    {
        SnapshotDataModel snapshotDataModel = _snapshotStore.Get(id);

        return RestoreAggregateFromSnapshotInternal(snapshotDataModel, aggregate);
    }

    private async Task<int> RestoreAggregateFromSnapshotAsync<T>(Guid id, T aggregate,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        SnapshotDataModel snapshotDataModel = await _snapshotStore.GetAsync(id, cancellationToken);

        return RestoreAggregateFromSnapshotInternal(snapshotDataModel, aggregate);
    }

    private SnapshotDataModel TakeSnapshot(AggregateRoot aggregate, bool userSerializer, bool force)
    {
        if (!force && !_snapshotStrategy.ShouldTakeSnapShot(aggregate))
        {
            return null;
        }

        int aggregateVersion = aggregate.AggregateVersion + aggregate.GetUncommittedChanges().Length;
        object aggregateState = userSerializer ? _serializer.Serialize(aggregate.State) : aggregate.State;
        Snapshot snapshot = new(aggregate.AggregateIdentifier, aggregateVersion, aggregateState);
        SnapshotDataModel snapshotDataModel = _snapshotMapper.Map(snapshot);

        return snapshotDataModel;
    }

    private int RestoreAggregateFromSnapshotInternal(SnapshotDataModel snapshotDataModel, AggregateRoot aggregate)
    {
        if (snapshotDataModel == null)
        {
            return -1;
        }

        Snapshot snapshot = _snapshotMapper.Map(snapshotDataModel);

        aggregate.RestoreState(snapshot.AggregateState, _serializer);

        return snapshot.AggregateVersion;
    }
}