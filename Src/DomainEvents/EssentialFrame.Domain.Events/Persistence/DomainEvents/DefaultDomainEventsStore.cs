using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Interfaces;
using EssentialFrame.Domain.Events.Persistence.DomainEvents.Interfaces;

namespace EssentialFrame.Domain.Events.Persistence.DomainEvents;

internal sealed class DefaultDomainEventsStore : IDomainEventsStore
{
    private readonly ICache<Guid, AggregateRoot> _aggregateCache;
    private readonly IAggregateOfflineStorage _aggregateOfflineStorage;
    private readonly ICache<Guid, DomainEventDataModel> _eventsCache;

    public DefaultDomainEventsStore(ICache<Guid, DomainEventDataModel> eventsCache,
        ICache<Guid, AggregateRoot> aggregateCache, IAggregateOfflineStorage aggregateOfflineStorage)
    {
        _eventsCache = eventsCache ?? throw new ArgumentNullException(nameof(eventsCache));
        _aggregateCache = aggregateCache ?? throw new ArgumentNullException(nameof(aggregateCache));
        _aggregateOfflineStorage =
            aggregateOfflineStorage ?? throw new ArgumentNullException(nameof(aggregateOfflineStorage));
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

    public void Save(AggregateRoot aggregate, IEnumerable<DomainEventDataModel> events)
    {
        IEnumerable<KeyValuePair<Guid, DomainEventDataModel>> domainEventDataModels =
            events?.Select(v => new KeyValuePair<Guid, DomainEventDataModel>(v.EventIdentifier, v));

        _aggregateCache.Add(aggregate.AggregateIdentifier, aggregate);
        _eventsCache.AddMany(domainEventDataModels);
    }

    public async Task SaveAsync(AggregateRoot aggregate, IEnumerable<DomainEventDataModel> events,
        CancellationToken cancellationToken = default)
    {
        Save(aggregate, events);

        await Task.CompletedTask;
    }

    public void Box(Guid aggregateIdentifier)
    {
        AggregateRoot aggregate = _aggregateCache.Get(aggregateIdentifier);
        IReadOnlyCollection<DomainEventDataModel> events =
            _eventsCache.GetMany((_, v) => v.AggregateIdentifier == aggregateIdentifier);

        _aggregateOfflineStorage.Save(aggregate, events);
    }

    public async Task BoxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        AggregateRoot aggregate = _aggregateCache.Get(aggregateIdentifier);
        IReadOnlyCollection<DomainEventDataModel> events =
            _eventsCache.GetMany((_, v) => v.AggregateIdentifier == aggregateIdentifier);

        await _aggregateOfflineStorage.SaveAsync(aggregate, events, cancellationToken);
    }
}