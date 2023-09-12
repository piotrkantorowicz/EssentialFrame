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
using EssentialFrame.Domain.ValueObjects.Core;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Events.Persistence.Snapshots.Services;

public class
    SnapshotRepository<TAggregate, TAggregateIdentifier> : ISnapshotRepository<TAggregate, TAggregateIdentifier>
    where TAggregate : IAggregateRoot<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    private readonly IAggregateRepository<TAggregate, TAggregateIdentifier> _aggregateRepository;
    private readonly IAggregateStore _aggregateStore;
    private readonly ICache<Guid, TAggregate> _snapshotsCache;
    private readonly IDomainEventMapper<TAggregateIdentifier> _domainEventMapper;
    private readonly ISerializer _serializer;
    private readonly ISnapshotMapper<TAggregateIdentifier> _snapshotMapper;
    private readonly ISnapshotStore _snapshotStore;
    private readonly ISnapshotStrategy<TAggregate, TAggregateIdentifier> _snapshotStrategy;

    public SnapshotRepository(IAggregateStore aggregateStore,
        IAggregateRepository<TAggregate, TAggregateIdentifier> aggregateRepository, ISnapshotStore snapshotStore,
        ISnapshotStrategy<TAggregate, TAggregateIdentifier> snapshotStrategy, ISerializer serializer,
        ICache<Guid, TAggregate> snapshotsCache, ISnapshotMapper<TAggregateIdentifier> snapshotMapper,
        IDomainEventMapper<TAggregateIdentifier> domainEventMapper)
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

    public TAggregate Get(TAggregateIdentifier aggregateIdentifier) 
    {
        TAggregate snapshot = _snapshotsCache.Get(aggregateIdentifier.Value);

        if (snapshot != null)
        {
            return snapshot;
        }

        TAggregate aggregate =
            GenericAggregateFactory<TAggregate, TAggregateIdentifier>.CreateAggregate(aggregateIdentifier);
        int snapshotVersion = RestoreAggregateFromSnapshot(aggregateIdentifier, aggregate);

        if (snapshotVersion == -1)
        {
            return _aggregateRepository.Get(aggregateIdentifier);
        }

        IReadOnlyCollection<DomainEventDataModel> allEvents =
            _aggregateStore.Get(aggregateIdentifier.Value, snapshotVersion);

        IEnumerable<IDomainEvent<TAggregateIdentifier>> events =
            _domainEventMapper.Map(allEvents.Where(desc => desc.AggregateVersion <= snapshotVersion));

        aggregate.Rehydrate(events);

        return aggregate;
    }

    public async Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier,
        CancellationToken cancellationToken = default)
    {
        TAggregate snapshot = _snapshotsCache.Get(aggregateIdentifier.Value);

        if (snapshot != null)
        {
            return snapshot;
        }

        TAggregate aggregate =
            GenericAggregateFactory<TAggregate, TAggregateIdentifier>.CreateAggregate(aggregateIdentifier);
        int snapshotVersion =
            await RestoreAggregateFromSnapshotAsync(aggregateIdentifier, aggregate, cancellationToken);

        if (snapshotVersion == -1)
        {
            return await _aggregateRepository.GetAsync(aggregateIdentifier, cancellationToken);
        }

        IReadOnlyCollection<DomainEventDataModel> allEvents = await _aggregateStore.GetAsync(aggregateIdentifier.Value,
            snapshotVersion, cancellationToken);

        IEnumerable<IDomainEvent<TAggregateIdentifier>> events =
            _domainEventMapper.Map(allEvents.Where(desc => desc.AggregateVersion <= snapshotVersion));

        aggregate.Rehydrate(events);
        

        return aggregate;
    }

    public IDomainEvent<TAggregateIdentifier>[] Save(TAggregate aggregate, int? version = null, int? timeout = null) 
    {
        if (timeout is > 0)
        {
            _snapshotsCache.Add(aggregate.AggregateIdentifier.Value, aggregate, timeout.GetValueOrDefault(), true);
        }

        TakeSnapshot(aggregate, false, false);

        return _aggregateRepository.Save(aggregate, version);
    }

    public async Task<IDomainEvent<TAggregateIdentifier>[]> SaveAsync(TAggregate aggregate, int? version = null,
        int? timeout = null,
        CancellationToken cancellationToken = default) 
    {
        if (timeout is > 0)
        {
            _snapshotsCache.Add(aggregate.AggregateIdentifier.Value, aggregate, timeout.GetValueOrDefault(), true);
        }

        TakeSnapshot(aggregate, false, false);

        return await _aggregateRepository.SaveAsync(aggregate, version, cancellationToken);
    }

    public void Box(TAggregate aggregate, bool useSerializer = false) 
    {
        SnapshotDataModel snapshotDataModel = TakeSnapshot(aggregate, useSerializer, true);

        _snapshotStore.Save(snapshotDataModel);
        _snapshotStore.Box(aggregate.AggregateIdentifier.Value);
        _aggregateStore.Box(aggregate.AggregateIdentifier.Value);

        _snapshotsCache.Remove(aggregate.AggregateIdentifier.Value);
    }

    public async Task BoxAsync(TAggregate aggregate, bool useSerializer = false,
        CancellationToken cancellationToken = default) 
    {
        SnapshotDataModel snapshotDataModel = TakeSnapshot(aggregate, useSerializer, true);

        await _snapshotStore.SaveAsync(snapshotDataModel, cancellationToken);
        await _snapshotStore.BoxAsync(aggregate.AggregateIdentifier.Value, cancellationToken);
        await _aggregateStore.BoxAsync(aggregate.AggregateIdentifier.Value, cancellationToken);

        _snapshotsCache.Remove(aggregate.AggregateIdentifier.Value);
    }

    public TAggregate Unbox(TAggregateIdentifier aggregateId, bool useSerializer = false)
    {
        SnapshotDataModel snapshotDataModel = _snapshotStore.Unbox(aggregateId.Value);
        Snapshot<TAggregateIdentifier> snapshot = _snapshotMapper.Map(snapshotDataModel);
        TAggregate aggregate =
            GenericAggregateFactory<TAggregate, TAggregateIdentifier>.CreateAggregate(aggregateId,
                snapshot.AggregateVersion);

        aggregate.RestoreState(snapshot.AggregateState, useSerializer ? _serializer : null);

        return aggregate;
    }

    public async Task<TAggregate> UnboxAsync(TAggregateIdentifier aggregateIdentifier, bool useSerializer = false,
        CancellationToken cancellationToken = default) 
    {
        SnapshotDataModel snapshotDataModel =
            await _snapshotStore.UnboxAsync(aggregateIdentifier.Value, cancellationToken);
        Snapshot<TAggregateIdentifier> snapshot = _snapshotMapper.Map(snapshotDataModel);
        TAggregate aggregate =
            GenericAggregateFactory<TAggregate, TAggregateIdentifier>.CreateAggregate(aggregateIdentifier,
                snapshot.AggregateVersion);

        aggregate.RestoreState(snapshot.AggregateState, useSerializer ? _serializer : null);

        return aggregate;
    }

    private int RestoreAggregateFromSnapshot(TAggregateIdentifier aggregateIdentifier, TAggregate aggregate)
    {
        SnapshotDataModel snapshotDataModel = _snapshotStore.Get(aggregateIdentifier.Value);

        return RestoreAggregateFromSnapshotInternal(snapshotDataModel, aggregate);
    }

    private async Task<int> RestoreAggregateFromSnapshotAsync(TAggregateIdentifier aggregateIdentifier,
        TAggregate aggregate,
        CancellationToken cancellationToken = default)
    {
        SnapshotDataModel snapshotDataModel =
            await _snapshotStore.GetAsync(aggregateIdentifier.Value, cancellationToken);

        return RestoreAggregateFromSnapshotInternal(snapshotDataModel, aggregate);
    }

    private SnapshotDataModel TakeSnapshot(TAggregate aggregate, bool userSerializer, bool force)
    {
        if (!force && !_snapshotStrategy.ShouldTakeSnapShot(aggregate))
        {
            return null;
        }

        int aggregateVersion = aggregate.AggregateVersion + aggregate.GetUncommittedChanges().Length;
        object aggregateState = userSerializer ? _serializer.Serialize(aggregate.State) : aggregate.State;
        Snapshot<TAggregateIdentifier> snapshot = new(aggregate.AggregateIdentifier, aggregateVersion, aggregateState);
        SnapshotDataModel snapshotDataModel = _snapshotMapper.Map(snapshot);

        return snapshotDataModel;
    }

    private int RestoreAggregateFromSnapshotInternal(SnapshotDataModel snapshotDataModel, TAggregate aggregate)
    {
        if (snapshotDataModel == null)
        {
            return -1;
        }

        Snapshot<TAggregateIdentifier> snapshot = _snapshotMapper.Map(snapshotDataModel);

        aggregate.RestoreState(snapshot.AggregateState, _serializer);

        return snapshot.AggregateVersion;
    }
}