using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Events.Interfaces;

namespace EssentialFrame.Domain.Events.Events;

internal sealed class DefaultDomainEventsStore : IDomainEventsStore
{
    private readonly ICache<Guid, AggregateRoot> _aggregateCache;
    private readonly ICache<Guid, DomainEventDao> _eventsCache;
    private readonly IAggregateOfflineStorage _aggregateOfflineStorage;

    public DefaultDomainEventsStore(ICache<Guid, DomainEventDao> eventsCache,
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

    public IReadOnlyCollection<DomainEventDao> Get(Guid aggregateIdentifier, int version)
    {
        return _eventsCache.GetMany((_, v) =>
            v.AggregateIdentifier == aggregateIdentifier && v.AggregateVersion == version);
    }

    public async Task<IReadOnlyCollection<DomainEventDao>> GetAsync(Guid aggregateIdentifier, int version,
        CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(Get(aggregateIdentifier, version));
    }

    public IEnumerable<Guid> GetDeleted()
    {
        return _aggregateCache.GetMany((_, v) => v.IsDeleted).Select(v => v.AggregateIdentifier);
    }

    public async Task<IEnumerable<Guid>> GetDeletedAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(GetDeleted());
    }

    public void Save(AggregateRoot aggregate, IEnumerable<DomainEventDao> events)
    {
        _aggregateCache.Add(aggregate.AggregateIdentifier, aggregate);
        _eventsCache.AddMany(events.Select(v => new KeyValuePair<Guid, DomainEventDao>(v.EventIdentifier, v)));
    }

    public async Task SaveAsync(AggregateRoot aggregate, IEnumerable<DomainEventDao> events,
        CancellationToken cancellationToken = default)
    {
        Save(aggregate, events);

        await Task.CompletedTask;
    }

    public void Box(Guid aggregateIdentifier)
    {
        (AggregateRoot aggregate, IReadOnlyCollection<IDomainEvent> events) =
            GetAggregateAndEvents(aggregateIdentifier);

        _aggregateOfflineStorage.Save(aggregate, events);
    }

    public async Task BoxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        (AggregateRoot aggregate, IReadOnlyCollection<IDomainEvent> events) =
            GetAggregateAndEvents(aggregateIdentifier);

        await _aggregateOfflineStorage.SaveAsync(aggregate, events, cancellationToken);
    }

    private (AggregateRoot, IReadOnlyCollection<IDomainEvent>) GetAggregateAndEvents(Guid aggregateIdentifier)
    {
        AggregateRoot aggregate = _aggregateCache.Get(aggregateIdentifier);
        List<IDomainEvent> events = _eventsCache.GetMany((_, v) => v.AggregateIdentifier == aggregateIdentifier).Select(
            v =>
            {
                if (v.DomainEvent is IDomainEvent domainEvent)
                {
                    return domainEvent;
                }

                throw new InvalidCastException($"Unable to cast {v.DomainEvent.GetType()} to {nameof(IDomainEvent)}");
            }).ToList();

        return (aggregate, events);
    }
}