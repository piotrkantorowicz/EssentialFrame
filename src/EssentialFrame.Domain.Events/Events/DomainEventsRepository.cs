using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Events.Interfaces;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Events.Events;

public sealed class DomainEventsRepository : IDomainEventsRepository
{
    private readonly ISerializer _serializer;
    private readonly IDomainEventsStore _store;

    public DomainEventsRepository(IDomainEventsStore store, ISerializer serializer)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
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

        IDomainEvent[] events = aggregate.FlushUncommittedChanges();

        IEnumerable<DomainEventDao> eventDaos = events.Select(e => new DomainEventDao(e));

        _store.Save(aggregate, eventDaos);

        return events;
    }

    public async Task<IDomainEvent[]> SaveAsync<T>(T aggregate, int? version = null,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        if (version != null &&
            await _store.ExistsAsync(aggregate.AggregateIdentifier, version.Value, cancellationToken))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        IDomainEvent[] events = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDao> eventDaos = events.Select(e => new DomainEventDao(e));

        await _store.SaveAsync(aggregate, eventDaos, cancellationToken);

        return events;
    }

    public IDomainEvent ConvertToEvent(DomainEventDao domainEventDao)
    {
        object @event = domainEventDao.DomainEvent;

        if (@event is string serializedEvent)
        {
            IDomainEvent deserialized = _serializer.Deserialize<IDomainEvent>(serializedEvent, Type.GetType(domainEventDao.EventClass));

            if (deserialized is null)
            {
                throw new UnknownEventTypeException(domainEventDao.EventType);
            }
        }

        IDomainEvent castedDomainEvent = @event as IDomainEvent;

        return castedDomainEvent;
    }

    private T Rehydrate<T>(Guid id) where T : AggregateRoot
    {
        IReadOnlyCollection<DomainEventDao> events = _store.Get(id, -1);
        T aggregate = RehydrateInternal<T>(id, events);

        return aggregate;
    }

    private async Task<T> RehydrateAsync<T>(Guid id, CancellationToken cancellationToken = default)
        where T : AggregateRoot
    {
        IReadOnlyCollection<DomainEventDao> events = await _store.GetAsync(id, -1, cancellationToken);

        T aggregate = RehydrateInternal<T>(id, events);

        return aggregate;
    }

    private T RehydrateInternal<T>(Guid id, IEnumerable<DomainEventDao> eventsData) where T : AggregateRoot
    {
        List<IDomainEvent> events = eventsData.Select(ConvertToEvent).ToList();

        if (!events.Any())
        {
            throw new AggregateNotFoundException(typeof(T), id);
        }

        T aggregate = AggregateFactory<T>.CreateAggregate();
        aggregate.Rehydrate(events);

        return aggregate;
    }
}