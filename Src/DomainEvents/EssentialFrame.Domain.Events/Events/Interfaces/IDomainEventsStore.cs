using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.Events.Events.Interfaces;

public interface IDomainEventsStore
{
    bool Exists(Guid aggregate);

    Task<bool> ExistsAsync(Guid aggregate, CancellationToken cancellationToken = default);

    bool Exists(Guid aggregate, int version);

    Task<bool> ExistsAsync(Guid aggregate, int version, CancellationToken cancellationToken = default);

    IReadOnlyCollection<DomainEventDao> Get(Guid aggregate, int version);

    Task<IReadOnlyCollection<DomainEventDao>> GetAsync(Guid aggregate, int version,
        CancellationToken cancellationToken = default);

    IEnumerable<Guid> GetExpired(DateTimeOffset at);

    Task<IEnumerable<Guid>> GetExpiredAsync(DateTimeOffset at, CancellationToken cancellationToken = default);

    void Save(AggregateRoot aggregate, IEnumerable<DomainEventDao> events);

    Task SaveAsync(AggregateRoot aggregate, IEnumerable<DomainEventDao> events,
        CancellationToken cancellationToken = default);

    void Box(Guid aggregate);

    Task BoxAsync(Guid aggregate, CancellationToken cancellationToken = default);
}