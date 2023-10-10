using System.Text;
using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Persistence.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services;

internal sealed class DefaultEventSourcingAggregateStore : IEventSourcingAggregateStore
{
    private readonly ICache<string, EventSourcingAggregateDataModel> _aggregateCache;
    private readonly ICache<Guid, DomainEventDataModel> _eventsCache;
    private readonly IEventSourcingAggregateOfflineStorage _eventSourcingAggregateOfflineStorage;

    public DefaultEventSourcingAggregateStore(ICache<Guid, DomainEventDataModel> eventsCache,
        ICache<string, EventSourcingAggregateDataModel> aggregateCache,
        IEventSourcingAggregateOfflineStorage eventSourcingAggregateOfflineStorage)
    {
        _eventsCache = eventsCache ?? throw new ArgumentNullException(nameof(eventsCache));
        _aggregateCache = aggregateCache ?? throw new ArgumentNullException(nameof(aggregateCache));
        
        _eventSourcingAggregateOfflineStorage = eventSourcingAggregateOfflineStorage ??
                                                throw new ArgumentNullException(
                                                    nameof(eventSourcingAggregateOfflineStorage));
    }

    public bool Exists(string aggregateIdentifier)
    {
        return _aggregateCache.Exists(aggregateIdentifier);
    }

    public async Task<bool> ExistsAsync(string aggregateIdentifier, CancellationToken cancellationToken)
    {
        return await Task.FromResult(_aggregateCache.Exists(aggregateIdentifier));
    }

    public bool Exists(string aggregateIdentifier, int version)
    {
        return _aggregateCache.Exists((_, v) =>
            v.AggregateIdentifier == aggregateIdentifier && v.AggregateVersion == version);
    }

    public async Task<bool> ExistsAsync(string aggregateIdentifier, int version, CancellationToken cancellationToken)
    {
        return await Task.FromResult(Exists(aggregateIdentifier, version));
    }

    public EventSourcingAggregateDataModel Get(string aggregateIdentifier)
    {
        return _aggregateCache.Get(aggregateIdentifier);
    }

    public async Task<EventSourcingAggregateDataModel> GetAsync(string aggregateIdentifier,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(Get(aggregateIdentifier));
    }

    public IReadOnlyCollection<DomainEventDataModel> Get(string aggregateIdentifier, int version)
    {
        return _eventsCache.GetMany((_, v) =>
            v.AggregateIdentifier == aggregateIdentifier && v.AggregateVersion >= version);
    }

    public async Task<IReadOnlyCollection<DomainEventDataModel>> GetAsync(string aggregateIdentifier, int version,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(Get(aggregateIdentifier, version));
    }

    public void Save(EventSourcingAggregateDataModel eventSourcingAggregate, IEnumerable<DomainEventDataModel> events)
    {
        IEnumerable<KeyValuePair<Guid, DomainEventDataModel>> domainEventDataModels =
            events?.Select(v => new KeyValuePair<Guid, DomainEventDataModel>(v.EventIdentifier, v));

        _aggregateCache.Add(eventSourcingAggregate.AggregateIdentifier, eventSourcingAggregate);
        _eventsCache.AddMany(domainEventDataModels);
    }

    public async Task SaveAsync(EventSourcingAggregateDataModel eventSourcingAggregate,
        IEnumerable<DomainEventDataModel> events, CancellationToken cancellationToken)
    {
        Save(eventSourcingAggregate, events);

        await Task.CompletedTask;
    }

    public void Box(string aggregateIdentifier, Encoding encoding)
    {
        EventSourcingAggregateDataModel eventSourcingAggregate = _aggregateCache.Get(aggregateIdentifier);

        if (eventSourcingAggregate is null)
        {
            throw new AggregateNotFoundException(typeof(EventSourcingAggregateDataModel), aggregateIdentifier);
        }

        IReadOnlyCollection<DomainEventDataModel> events =
            _eventsCache.GetMany((_, v) => v.AggregateIdentifier == aggregateIdentifier);

        
        
        _eventSourcingAggregateOfflineStorage.Save(eventSourcingAggregate, events, encoding ?? Encoding.Unicode);
    }

    public async Task BoxAsync(string aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken)
    {
        EventSourcingAggregateDataModel eventSourcingAggregate = _aggregateCache.Get(aggregateIdentifier);

        IReadOnlyCollection<DomainEventDataModel> events =
            _eventsCache.GetMany((_, v) => v.AggregateIdentifier == aggregateIdentifier);

        await _eventSourcingAggregateOfflineStorage.SaveAsync(eventSourcingAggregate, events,
            encoding ?? Encoding.Unicode, cancellationToken);
    }

    public EventSourcingAggregateWithEventsModel Unbox(string aggregateIdentifier, Encoding encoding)
    {
        return _eventSourcingAggregateOfflineStorage.Get(aggregateIdentifier, encoding);
    }

    public async Task<EventSourcingAggregateWithEventsModel> UnboxAsync(string aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken)
    {
        return await _eventSourcingAggregateOfflineStorage.GetAsync(aggregateIdentifier, encoding, cancellationToken);
    }
}