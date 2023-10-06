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

    public TAggregate Get(TAggregateIdentifier aggregateIdentifier)
    {
        TAggregate snapshot = _snapshotsCache.Get(aggregateIdentifier);

        if (snapshot != null)
        {
            return snapshot;
        }

        TAggregate aggregate =
            EventSourcingGenericAggregateFactory<TAggregate, TAggregateIdentifier, TType>.CreateAggregate(
                aggregateIdentifier);

        int snapshotVersion = RestoreAggregateFromSnapshot(aggregateIdentifier, aggregate);

        if (snapshotVersion == -1)
        {
            return _eventSourcingAggregateRepository.Get(aggregateIdentifier);
        }

        IReadOnlyCollection<DomainEventDataModel> allEvents =
            _eventSourcingAggregateStore.Get(aggregateIdentifier, snapshotVersion);

        IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> events =
            _domainEventMapper.Map(allEvents.Where(desc => desc.AggregateVersion <= snapshotVersion));

        aggregate.Rehydrate(events);

        return aggregate;
    }

    public async Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier,
        CancellationToken cancellationToken = default)
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
            await RestoreAggregateFromSnapshotAsync(aggregateIdentifier, aggregate, cancellationToken);

        if (snapshotVersion == -1)
        {
            return await _eventSourcingAggregateRepository.GetAsync(aggregateIdentifier, cancellationToken);
        }

        IReadOnlyCollection<DomainEventDataModel> allEvents =
            await _eventSourcingAggregateStore.GetAsync(aggregateIdentifier, snapshotVersion, cancellationToken);

        IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> events =
            _domainEventMapper.Map(allEvents.Where(desc => desc.AggregateVersion <= snapshotVersion));

        aggregate.Rehydrate(events);


        return aggregate;
    }

    public IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate, int? version = null,
        int? timeout = null)
    {
        if (timeout is > 0)
        {
            _snapshotsCache.Add(aggregate.AggregateIdentifier, aggregate, timeout.GetValueOrDefault(), true);
        }

        TakeSnapshot(aggregate, false, false);

        return _eventSourcingAggregateRepository.Save(aggregate, version);
    }

    public async Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate, int? version = null,
        int? timeout = null, CancellationToken cancellationToken = default)
    {
        if (timeout is > 0)
        {
            _snapshotsCache.Add(aggregate.AggregateIdentifier, aggregate, timeout.GetValueOrDefault(), true);
        }

        TakeSnapshot(aggregate, false, false);

        return await _eventSourcingAggregateRepository.SaveAsync(aggregate, version, cancellationToken);
    }

    public void Box(TAggregate aggregate, bool useSerializer = false)
    {
        SnapshotDataModel snapshotDataModel = TakeSnapshot(aggregate, useSerializer, true);

        _snapshotStore.Save(snapshotDataModel);
        _snapshotStore.Box(aggregate.AggregateIdentifier);
        _eventSourcingAggregateStore.Box(aggregate.AggregateIdentifier);
        _snapshotsCache.Remove(aggregate.AggregateIdentifier);
    }

    public async Task BoxAsync(TAggregate aggregate, bool useSerializer = false,
        CancellationToken cancellationToken = default)
    {
        SnapshotDataModel snapshotDataModel = TakeSnapshot(aggregate, useSerializer, true);

        await _snapshotStore.SaveAsync(snapshotDataModel, cancellationToken);
        await _snapshotStore.BoxAsync(aggregate.AggregateIdentifier, cancellationToken);
        await _eventSourcingAggregateStore.BoxAsync(aggregate.AggregateIdentifier, cancellationToken);
        _snapshotsCache.Remove(aggregate.AggregateIdentifier);
    }

    public TAggregate Unbox(TAggregateIdentifier aggregateIdentifier, bool useSerializer = false)
    {
        SnapshotDataModel snapshotDataModel = _snapshotStore.Unbox(aggregateIdentifier);
        Snapshot<TAggregateIdentifier, TType> snapshot = _snapshotMapper.Map(snapshotDataModel);
        
        TAggregate aggregate =
            EventSourcingGenericAggregateFactory<TAggregate, TAggregateIdentifier, TType>.CreateAggregate(
                aggregateIdentifier,
                snapshot.AggregateVersion);

        aggregate.RestoreState(snapshot.AggregateState, useSerializer ? _serializer : null);

        return aggregate;
    }

    public async Task<TAggregate> UnboxAsync(TAggregateIdentifier aggregateIdentifier, bool useSerializer = false,
        CancellationToken cancellationToken = default)
    {
        SnapshotDataModel snapshotDataModel = await _snapshotStore.UnboxAsync(aggregateIdentifier, cancellationToken);
        Snapshot<TAggregateIdentifier, TType> snapshot = _snapshotMapper.Map(snapshotDataModel);
        
        TAggregate aggregate =
            EventSourcingGenericAggregateFactory<TAggregate, TAggregateIdentifier, TType>.CreateAggregate(
                aggregateIdentifier,
                snapshot.AggregateVersion);

        aggregate.RestoreState(snapshot.AggregateState, useSerializer ? _serializer : null);

        return aggregate;
    }

    private int RestoreAggregateFromSnapshot(TAggregateIdentifier aggregateIdentifier, TAggregate aggregate)
    {
        SnapshotDataModel snapshotDataModel = _snapshotStore.Get(aggregateIdentifier);

        return RestoreAggregateFromSnapshotInternal(snapshotDataModel, aggregate);
    }

    private async Task<int> RestoreAggregateFromSnapshotAsync(TAggregateIdentifier aggregateIdentifier,
        TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        SnapshotDataModel snapshotDataModel = await _snapshotStore.GetAsync(aggregateIdentifier, cancellationToken);

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
        Snapshot<TAggregateIdentifier, TType> snapshot = new(aggregate.AggregateIdentifier, aggregateVersion,
            aggregateState);
        SnapshotDataModel snapshotDataModel = _snapshotMapper.Map(snapshot);

        return snapshotDataModel;
    }

    private int RestoreAggregateFromSnapshotInternal(SnapshotDataModel snapshotDataModel, TAggregate aggregate)
    {
        if (snapshotDataModel == null)
        {
            return -1;
        }

        Snapshot<TAggregateIdentifier, TType> snapshot = _snapshotMapper.Map(snapshotDataModel);

        aggregate.RestoreState(snapshot.AggregateState, _serializer);

        return snapshot.AggregateVersion;
    }
}