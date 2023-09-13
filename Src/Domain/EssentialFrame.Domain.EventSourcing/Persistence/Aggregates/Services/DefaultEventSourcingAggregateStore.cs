using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services;

internal sealed class DefaultEventSourcingAggregateStore : IEventSourcingAggregateStore
{
    private readonly ICache<Guid, EventSourcingAggregateDataModel> _aggregateCache;
    private readonly ICache<Guid, DomainEventDataModel> _eventsCache;
    private readonly IEventSourcingAggregateOfflineStorage _eventSourcingAggregateOfflineStorage;

    public DefaultEventSourcingAggregateStore(ICache<Guid, DomainEventDataModel> eventsCache,
        ICache<Guid, EventSourcingAggregateDataModel> aggregateCache,
        IEventSourcingAggregateOfflineStorage eventSourcingAggregateOfflineStorage)
    {
        _eventsCache = eventsCache ?? throw new ArgumentNullException(nameof(eventsCache));
        _aggregateCache = aggregateCache ?? throw new ArgumentNullException(nameof(aggregateCache));
        _eventSourcingAggregateOfflineStorage = eventSourcingAggregateOfflineStorage ??
                                                throw new ArgumentNullException(
                                                    nameof(eventSourcingAggregateOfflineStorage));
    }

    public bool Exists(Guid aggregateIdentifier)
    {
        return _aggregateCache.Exists(aggregateIdentifier);
    }

    public Task<bool> ExistsAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_aggregateCache.Exists(aggregateIdentifier));
    }

    public bool Exists(Guid aggregateIdentifier, int version)
    {
        return _aggregateCache.Exists((_, v) =>
            v.AggregateIdentifier == aggregateIdentifier && v.AggregateVersion == version);
    }

    public async Task<bool> ExistsAsync(Guid aggregateIdentifier, int version,
        CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Exists(aggregateIdentifier, version));
    }

    public EventSourcingAggregateDataModel Get(Guid aggregateIdentifier)
    {
        return _aggregateCache.Get(aggregateIdentifier);
    }

    public async Task<EventSourcingAggregateDataModel> GetAsync(Guid aggregateIdentifier,
        CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Get(aggregateIdentifier));
    }

    public IReadOnlyCollection<DomainEventDataModel> Get(Guid aggregateIdentifier, int version)
    {
        return _eventsCache.GetMany((_, v) =>
            v.AggregateIdentifier == aggregateIdentifier && v.AggregateVersion >= version);
    }

    public async Task<IReadOnlyCollection<DomainEventDataModel>> GetAsync(Guid aggregateIdentifier, int version,
        CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Get(aggregateIdentifier, version));
    }

    public IEnumerable<Guid> GetDeleted()
    {
        return _aggregateCache.GetMany((_, v) => v.IsDeleted)?.Select(v => v.AggregateIdentifier);
    }

    public async Task<IEnumerable<Guid>> GetDeletedAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(GetDeleted());
    }

    public void Save(EventSourcingAggregateDataModel eventSourcingAggregate, IEnumerable<DomainEventDataModel> events)
    {
        IEnumerable<KeyValuePair<Guid, DomainEventDataModel>> domainEventDataModels =
            events?.Select(v => new KeyValuePair<Guid, DomainEventDataModel>(v.EventIdentifier, v));

        _aggregateCache.Add(eventSourcingAggregate.AggregateIdentifier, eventSourcingAggregate);
        _eventsCache.AddMany(domainEventDataModels);
    }

    public async Task SaveAsync(EventSourcingAggregateDataModel eventSourcingAggregate,
        IEnumerable<DomainEventDataModel> events, CancellationToken cancellationToken = default)
    {
        Save(eventSourcingAggregate, events);

        await Task.CompletedTask;
    }

    public void Box(Guid aggregateIdentifier)
    {
        EventSourcingAggregateDataModel eventSourcingAggregate = _aggregateCache.Get(aggregateIdentifier);

        IReadOnlyCollection<DomainEventDataModel> events =
            _eventsCache.GetMany((_, v) => v.AggregateIdentifier == aggregateIdentifier);

        _eventSourcingAggregateOfflineStorage.Save(eventSourcingAggregate, events);
    }

    public async Task BoxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        EventSourcingAggregateDataModel eventSourcingAggregate = _aggregateCache.Get(aggregateIdentifier);

        IReadOnlyCollection<DomainEventDataModel> events =
            _eventsCache.GetMany((_, v) => v.AggregateIdentifier == aggregateIdentifier);

        await _eventSourcingAggregateOfflineStorage.SaveAsync(eventSourcingAggregate, events, cancellationToken);
    }
}