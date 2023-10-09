using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.EventSourcing.Core.Factories;
using EssentialFrame.Domain.EventSourcing.Core.Snapshots;
using EssentialFrame.Domain.EventSourcing.Core.Snapshots.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services.Interfaces;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services;

internal sealed class
    SnapshotRepository<TAggregate, TAggregateIdentifier, TType> : ISnapshotRepository<TAggregate, TAggregateIdentifier,
        TType> where TAggregate : class, IEventSourcingAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    private readonly IEventSourcingAggregateRepository<TAggregate, TAggregateIdentifier, TType>
        _eventSourcingAggregateRepository;

    private readonly IEventSourcingAggregateStore _eventSourcingAggregateStore;
    private readonly ICache<TAggregateIdentifier, TAggregate> _snapshotsCache;
    private readonly IDomainEventMapper<TAggregateIdentifier, TType> _domainEventMapper;
    private readonly ISerializer _serializer;
    private readonly ISnapshotMapper<TAggregateIdentifier, TType> _snapshotMapper;
    private readonly ISnapshotStore _snapshotStore;
    private readonly ISnapshotStrategy<TAggregate, TAggregateIdentifier, TType> _snapshotStrategy;

    public SnapshotRepository(IEventSourcingAggregateStore eventSourcingAggregateStore,
        IEventSourcingAggregateRepository<TAggregate, TAggregateIdentifier, TType> eventSourcingAggregateRepository,
        ISnapshotStore snapshotStore, ISnapshotStrategy<TAggregate, TAggregateIdentifier, TType> snapshotStrategy,
        ISerializer serializer, ICache<TAggregateIdentifier, TAggregate> snapshotsCache,
        ISnapshotMapper<TAggregateIdentifier, TType> snapshotMapper,
        IDomainEventMapper<TAggregateIdentifier, TType> domainEventMapper)
    {
        _eventSourcingAggregateStore = eventSourcingAggregateStore ??
                                       throw new ArgumentNullException(nameof(eventSourcingAggregateStore));

        _eventSourcingAggregateRepository = eventSourcingAggregateRepository ??
                                            throw new ArgumentNullException(nameof(eventSourcingAggregateRepository));

        _snapshotStore = snapshotStore ?? throw new ArgumentNullException(nameof(snapshotStore));
        _snapshotStrategy = snapshotStrategy ?? throw new ArgumentNullException(nameof(snapshotStrategy));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _snapshotsCache = snapshotsCache ?? throw new ArgumentNullException(nameof(snapshotsCache));
        _snapshotMapper = snapshotMapper ?? throw new ArgumentNullException(nameof(snapshotMapper));
        _domainEventMapper = domainEventMapper ?? throw new ArgumentNullException(nameof(domainEventMapper));
    }

    public TAggregate Get(TAggregateIdentifier aggregateIdentifier, bool useSerializer)
    {
        TAggregate snapshot = _snapshotsCache.Get(aggregateIdentifier);

        if (snapshot != null)
        {
            return snapshot;
        }

        TAggregate aggregate =
            EventSourcingGenericAggregateFactory<TAggregate, TAggregateIdentifier, TType>.CreateAggregate(
                aggregateIdentifier);

        int snapshotVersion = RestoreAggregateFromSnapshot(aggregateIdentifier, aggregate, useSerializer);

        if (snapshotVersion == -1)
        {
            return _eventSourcingAggregateRepository.Get(aggregateIdentifier);
        }

        IReadOnlyCollection<DomainEventDataModel> uncommittedChangesDao = _eventSourcingAggregateStore
            .Get(aggregateIdentifier, snapshotVersion).Where(desc => desc.AggregateVersion <= snapshotVersion).ToList();

        IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> uncommittedChanges =
            _domainEventMapper.Map(uncommittedChangesDao);

        aggregate.Rehydrate(uncommittedChanges);

        return aggregate;
    }

    public async Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier, bool useSerializer,
        CancellationToken cancellationToken)
    {
        TAggregate snapshot = _snapshotsCache.Get(aggregateIdentifier);

        if (snapshot != null)
        {
            return snapshot;
        }

        TAggregate aggregate =
            EventSourcingGenericAggregateFactory<TAggregate, TAggregateIdentifier, TType>.CreateAggregate(
                aggregateIdentifier);

        int snapshotVersion =
            await RestoreAggregateFromSnapshotAsync(aggregateIdentifier, aggregate, useSerializer, cancellationToken);

        if (snapshotVersion == -1)
        {
            return await _eventSourcingAggregateRepository.GetAsync(aggregateIdentifier, cancellationToken);
        }

        IReadOnlyCollection<DomainEventDataModel> uncommittedChangesDao =
            (await _eventSourcingAggregateStore.GetAsync(aggregateIdentifier, snapshotVersion, cancellationToken))
            .Where(desc => desc.AggregateVersion <= snapshotVersion).ToList();

        IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> uncommittedChanges =
            _domainEventMapper.Map(uncommittedChangesDao);

        aggregate.Rehydrate(uncommittedChanges);


        return aggregate;
    }

    public IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate)
    {
        return SaveInternal(aggregate, null, null);
    }

    public IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate, int version)
    {
        return SaveInternal(aggregate, version, null);
    }

    public IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate, int version, int timeout)
    {
        return SaveInternal(aggregate, version, timeout);
    }

    public async Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate,
        CancellationToken cancellationToken)
    {
        return await SaveInternalAsync(aggregate, null, null, cancellationToken);
    }

    public async Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate, int version,
        CancellationToken cancellationToken)
    {
        return await SaveInternalAsync(aggregate, version, null, cancellationToken);
    }

    public async Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate, int version,
        int timeout, CancellationToken cancellationToken)
    {
        return await SaveInternalAsync(aggregate, version, timeout, cancellationToken);
    }

    public void Box(TAggregate aggregate, bool useSerializer)
    {
        SnapshotDataModel snapshotDataModel = TakeSnapshot(aggregate, useSerializer, true);

        _snapshotStore.Save(snapshotDataModel);
        _snapshotStore.Box(aggregate.AggregateIdentifier);
        _eventSourcingAggregateStore.Box(aggregate.AggregateIdentifier);
        _snapshotsCache.Remove(aggregate.AggregateIdentifier);
    }

    public async Task BoxAsync(TAggregate aggregate, bool useSerializer, CancellationToken cancellationToken)
    {
        SnapshotDataModel snapshotDataModel = TakeSnapshot(aggregate, useSerializer, true);

        await _snapshotStore.SaveAsync(snapshotDataModel, cancellationToken);
        await _snapshotStore.BoxAsync(aggregate.AggregateIdentifier, cancellationToken);
        await _eventSourcingAggregateStore.BoxAsync(aggregate.AggregateIdentifier, cancellationToken);
        _snapshotsCache.Remove(aggregate.AggregateIdentifier);
    }

    public TAggregate Unbox(TAggregateIdentifier aggregateIdentifier, bool useSerializer)
    {
        SnapshotDataModel snapshotDataModel = _snapshotStore.Unbox(aggregateIdentifier);
        Snapshot<TAggregateIdentifier, TType> snapshot = _snapshotMapper.Map(snapshotDataModel);

        TAggregate aggregate =
            EventSourcingGenericAggregateFactory<TAggregate, TAggregateIdentifier, TType>.CreateAggregate(
                aggregateIdentifier, snapshot.AggregateVersion);

        return RestoreAggregateStateInternal(aggregate, snapshot, useSerializer);
    }

    public async Task<TAggregate> UnboxAsync(TAggregateIdentifier aggregateIdentifier, bool useSerializer,
        CancellationToken cancellationToken)
    {
        SnapshotDataModel snapshotDataModel = await _snapshotStore.UnboxAsync(aggregateIdentifier, cancellationToken);
        Snapshot<TAggregateIdentifier, TType> snapshot = _snapshotMapper.Map(snapshotDataModel);

        TAggregate aggregate =
            EventSourcingGenericAggregateFactory<TAggregate, TAggregateIdentifier, TType>.CreateAggregate(
                aggregateIdentifier, snapshot.AggregateVersion);

        return RestoreAggregateStateInternal(aggregate, snapshot, useSerializer);
    }

    private IDomainEvent<TAggregateIdentifier, TType>[] SaveInternal(TAggregate aggregate, int? version, int? timeout)
    {
        if (timeout is > 0)
        {
            _snapshotsCache.Add(aggregate.AggregateIdentifier, aggregate, timeout.GetValueOrDefault(), true);
        }

        TakeSnapshot(aggregate, false, false);

        return version.HasValue
            ? _eventSourcingAggregateRepository.Save(aggregate, version.Value)
            : _eventSourcingAggregateRepository.Save(aggregate);
    }

    private async Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveInternalAsync(TAggregate aggregate,
        int? version, int? timeout, CancellationToken cancellationToken)
    {
        if (timeout is > 0)
        {
            _snapshotsCache.Add(aggregate.AggregateIdentifier, aggregate, timeout.GetValueOrDefault(), true);
        }

        TakeSnapshot(aggregate, false, false);

        return version.HasValue
            ? await _eventSourcingAggregateRepository.SaveAsync(aggregate, version.Value, cancellationToken)
            : await _eventSourcingAggregateRepository.SaveAsync(aggregate, cancellationToken);
    }

    private int RestoreAggregateFromSnapshot(TAggregateIdentifier aggregateIdentifier, TAggregate aggregate,
        bool useSerializer)
    {
        SnapshotDataModel snapshotDataModel = _snapshotStore.Get(aggregateIdentifier);

        return RestoreAggregateFromSnapshotInternal(snapshotDataModel, aggregate, useSerializer);
    }

    private async Task<int> RestoreAggregateFromSnapshotAsync(TAggregateIdentifier aggregateIdentifier,
        TAggregate aggregate, bool useSerializer, CancellationToken cancellationToken)
    {
        SnapshotDataModel snapshotDataModel = await _snapshotStore.GetAsync(aggregateIdentifier, cancellationToken);

        return RestoreAggregateFromSnapshotInternal(snapshotDataModel, aggregate, useSerializer);
    }

    private SnapshotDataModel TakeSnapshot(TAggregate aggregate, bool userSerializer, bool force)
    {
        if (!force && !_snapshotStrategy.ShouldTakeSnapShot(aggregate))
        {
            return null;
        }

        int aggregateVersion = aggregate.AggregateVersion + aggregate.GetUncommittedChanges().Length;
        object aggregateState = userSerializer ? _serializer.Serialize(aggregate.State) : aggregate.State;
        Snapshot<TAggregateIdentifier, TType> snapshot = new(aggregate.AggregateIdentifier, aggregateVersion,
            aggregateState);
        SnapshotDataModel snapshotDataModel = _snapshotMapper.Map(snapshot);

        return snapshotDataModel;
    }

    private int RestoreAggregateFromSnapshotInternal(SnapshotDataModel snapshotDataModel, TAggregate aggregate,
        bool useSerializer)
    {
        if (snapshotDataModel == null)
        {
            return -1;
        }

        Snapshot<TAggregateIdentifier, TType> snapshot = _snapshotMapper.Map(snapshotDataModel);

        return RestoreAggregateStateInternal(aggregate, snapshot, useSerializer).AggregateVersion;
    }

    private TAggregate RestoreAggregateStateInternal(TAggregate aggregate,
        Snapshot<TAggregateIdentifier, TType> snapshot, bool useSerializer)
    {
        if (useSerializer)
        {
            aggregate.RestoreState(snapshot.AggregateState.ToString(), snapshot.AggregateVersion, _serializer);
            return aggregate;
        }

        aggregate.RestoreState(snapshot.AggregateState, snapshot.AggregateVersion);
        return aggregate;
    }
}