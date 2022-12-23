using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events;

namespace EssentialFrame.Domain.EventsSourcing.Events.Interfaces;

public interface IEventRepository
{
    T Get<T>(Guid id)
        where T : AggregateRoot;

    Task<T> GetAsync<T>(Guid id, CancellationToken cancellationToken = default)
        where T : AggregateRoot;

    IEvent[] Save<T>(T aggregate, int? version = null)
        where T : AggregateRoot;

    Task<IEvent[]> SaveAsync<T>(T aggregate,
                                int? version = null,
                                CancellationToken cancellationToken = default)
        where T : AggregateRoot;

    IEvent ConvertToEvent(EventData eventData);
}
