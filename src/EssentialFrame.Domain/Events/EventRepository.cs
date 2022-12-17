using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Core;
using EssentialFrame.Domain.Exceptions;

namespace EssentialFrame.Domain.Events;

public class EventRepository : IEventRepository
{
    private readonly IEventStore _store;

    public EventRepository(IEventStore store) => _store = store ?? throw new ArgumentNullException(nameof(store));

    public T Get<T>(Guid aggregate)
        where T : AggregateRoot => Rehydrate<T>(aggregate);

    public IEvent[] Save<T>(T aggregate, int? version)
        where T : AggregateRoot
    {
        if (version != null &&
            _store.Exists(aggregate.AggregateIdentifier, version.Value))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        var events = aggregate.FlushUncommittedChanges();

        _store.Save(aggregate, events);

        return events;
    }

    public void Box<T>(T aggregate)
        where T : AggregateRoot
    {
        throw new NotImplementedException();
    }

    public T Unbox<T>(Guid aggregateId)
        where T : AggregateRoot =>
        throw new NotImplementedException();

    private T Rehydrate<T>(Guid id)
        where T : AggregateRoot
    {
        var events = _store.Get(id, -1);

        if (!events.Any())
        {
            throw new AggregateNotFoundException(typeof(T), id);
        }

        var aggregate = AggregateFactory<T>.CreateAggregate();
        aggregate.Rehydrate(events);
        return aggregate;
    }
}

