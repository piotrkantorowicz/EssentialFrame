using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Persistence.DomainEvents.Interfaces;
using EssentialFrame.Domain.Factories;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Events.Persistence.DomainEvents;

public sealed class DomainEventsRepository : IDomainEventsRepository
{
    private readonly ISerializer _serializer;
    private readonly IDomainEventsStore _store;
    private readonly IDomainEventMapper _domainEventMapper;

    public DomainEventsRepository(IDomainEventsStore store, ISerializer serializer,
        IDomainEventMapper domainEventMapper)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _domainEventMapper = domainEventMapper ?? throw new ArgumentNullException(nameof(domainEventMapper));
    }

    public T Get<T>(Guid aggregate) where T : AggregateRoot
    {
        return Rehydrate<T>(aggregate);
    }

    public Task<T> GetAsync<T>(Guid aggregate, CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        return RehydrateAsync<T>(aggregate, cancellationToken);
    }

    public IDomainEvent[] Save<T>(T aggregate, int? version) where T : AggregateRoot
    {
        if (version != null && _store.Exists(aggregate.AggregateIdentifier, version.Value))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        IDomainEvent[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = domainEvents.Select(e => _domainEventMapper.Map(e));

        _store.Save(aggregate, domainEventDataModels);

        return domainEvents;
    }

    public async Task<IDomainEvent[]> SaveAsync<T>(T aggregate, int? version = null,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        if (version != null &&
            await _store.ExistsAsync(aggregate.AggregateIdentifier, version.Value, cancellationToken))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        IDomainEvent[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = domainEvents.Select(e => _domainEventMapper.Map(e));

        await _store.SaveAsync(aggregate, domainEventDataModels, cancellationToken);

        return domainEvents;
    }

    private T Rehydrate<T>(Guid id) where T : AggregateRoot
    {
        IReadOnlyCollection<DomainEventDataModel> events = _store.Get(id, -1);
        T aggregate = RehydrateInternal<T>(id, events);

        return aggregate;
    }

    private async Task<T> RehydrateAsync<T>(Guid id, CancellationToken cancellationToken = default)
        where T : AggregateRoot
    {
        IReadOnlyCollection<DomainEventDataModel> events = await _store.GetAsync(id, -1, cancellationToken);

        T aggregate = RehydrateInternal<T>(id, events);

        return aggregate;
    }

    private T RehydrateInternal<T>(Guid id, IEnumerable<DomainEventDataModel> eventsData) where T : AggregateRoot
    {
        List<IDomainEvent> events = eventsData.Select(e => _domainEventMapper.Map(e)).ToList();

        if (!events.Any())
        {
            throw new AggregateNotFoundException(typeof(T), id);
        }

        T aggregate = GenericAggregateFactory<T>.CreateAggregate(id, 0);
        aggregate.Rehydrate(events);

        return aggregate;
    }
}