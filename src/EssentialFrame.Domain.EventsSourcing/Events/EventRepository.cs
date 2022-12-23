using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.EventsSourcing.Events.Interfaces;
using EssentialFrame.Domain.EventsSourcing.Exceptions;
using EssentialFrame.Serialization;

namespace EssentialFrame.Domain.EventsSourcing.Events;

public sealed class EventRepository : IEventRepository
{
    private readonly ISerializer _serializer;
    private readonly IEventStore _store;

    public EventRepository(IEventStore store, ISerializer serializer)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public T Get<T>(Guid aggregate)
        where T : AggregateRoot => Rehydrate<T>(aggregate);

    public Task<T> GetAsync<T>(Guid aggregate, CancellationToken cancellationToken = default)
        where T : AggregateRoot => RehydrateAsync<T>(aggregate, cancellationToken);

    public IEvent[] Save<T>(T aggregate, int? version)
        where T : AggregateRoot
    {
        if (version != null &&
            _store.Exists(aggregate.AggregateIdentifier, version.Value))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        var events = aggregate.FlushUncommittedChanges();

        var eventData = events.Select(e => new EventData(e));

        _store.Save(aggregate, eventData);

        return events;
    }

    public async Task<IEvent[]> SaveAsync<T>(T aggregate,
                                             int? version = null,
                                             CancellationToken cancellationToken = default)
        where T : AggregateRoot
    {
        if (version != null &&
            await _store.ExistsAsync(aggregate.AggregateIdentifier,
                                     version.Value,
                                     cancellationToken))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        var events = aggregate.FlushUncommittedChanges();
        var eventData = events.Select(e => new EventData(e));

        await _store.SaveAsync(aggregate,
                               eventData,
                               cancellationToken);

        return events;
    }

    public IEvent ConvertToEvent(EventData eventData)
    {
        var @event = eventData.Event;

        if (@event is string serializedEvent)
        {
            var deserialized = _serializer.Deserialize<IEvent>(serializedEvent, Type.GetType(eventData.EventClass));

            if (deserialized is null)
            {
                throw new UnknownEventTypeException(eventData.EventType);
            }
        }

        var castedEvent = @event as IEvent;

        return castedEvent;
    }

    private T Rehydrate<T>(Guid id)
        where T : AggregateRoot
    {
        var events = _store.Get(id, -1);
        var aggregate = RehydrateInternal<T>(id, events);

        return aggregate;
    }

    private async Task<T> RehydrateAsync<T>(Guid id, CancellationToken cancellationToken = default)
        where T : AggregateRoot
    {
        var events = await _store.GetAsync(id,
                                           -1,
                                           cancellationToken);

        var aggregate = RehydrateInternal<T>(id, events);

        return aggregate;
    }

    private T RehydrateInternal<T>(Guid id, IEnumerable<EventData> eventsData)
        where T : AggregateRoot
    {
        var events = eventsData.Select(ConvertToEvent).ToList();

        if (!events.Any())
        {
            throw new AggregateNotFoundException(typeof(T), id);
        }

        var aggregate = AggregateFactory<T>.CreateAggregate();
        aggregate.Rehydrate(events);

        return aggregate;
    }
}
