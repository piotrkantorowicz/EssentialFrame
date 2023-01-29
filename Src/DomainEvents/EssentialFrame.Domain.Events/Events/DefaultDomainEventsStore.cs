using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Events.Interfaces;

namespace EssentialFrame.Domain.Events.Events;

internal sealed class DefaultDomainEventsStore : IDomainEventsStore
{
    private readonly ICache<Guid, DomainEventDao> _eventsCache;

    public DefaultDomainEventsStore(ICache<Guid, DomainEventDao> eventsCache)
    {
        _eventsCache = eventsCache ?? throw new ArgumentNullException(nameof(eventsCache));
    }
    
    public bool Exists(Guid aggregate)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(Guid aggregate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public bool Exists(Guid aggregate, int version)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(Guid aggregate, int version, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<DomainEventDao> Get(Guid aggregate, int version)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<DomainEventDao>> GetAsync(Guid aggregate, int version, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Guid> GetExpired(DateTimeOffset at)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Guid>> GetExpiredAsync(DateTimeOffset at, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Save(AggregateRoot aggregate, IEnumerable<DomainEventDao> events)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(AggregateRoot aggregate, IEnumerable<DomainEventDao> events, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Box(Guid aggregate)
    {
        throw new NotImplementedException();
    }

    public Task BoxAsync(Guid aggregate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}