using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.EventsSourcing.Events.Interfaces;

public interface IEventStore
{
    bool Exists(Guid aggregate);

    Task<bool> ExistsAsync(Guid aggregate, CancellationToken cancellationToken = default);

    bool Exists(Guid aggregate, int version);

    Task<bool> ExistsAsync(Guid aggregate,
                           int version,
                           CancellationToken cancellationToken = default);

    IReadOnlyCollection<EventData> Get(Guid aggregate, int version);

    Task<IReadOnlyCollection<EventData>> GetAsync(Guid aggregate,
                                                  int version,
                                                  CancellationToken cancellationToken = default);

    IEnumerable<Guid> GetExpired(DateTimeOffset at);

    Task<IEnumerable<Guid>> GetExpiredAsync(DateTimeOffset at, CancellationToken cancellationToken = default);

    void Save(AggregateRoot aggregate, IEnumerable<EventData> events);

    Task SaveAsync(AggregateRoot aggregate,
                   IEnumerable<EventData> events,
                   CancellationToken cancellationToken = default);

    void Box(Guid aggregate);

    Task BoxAsync(Guid aggregate, CancellationToken cancellationToken = default);
}
