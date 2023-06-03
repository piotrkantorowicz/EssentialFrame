using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.Events.Events.Interfaces;

public interface IDomainEventsStore
{
    bool Exists(Guid aggregateIdentifier);

    Task<bool> ExistsAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);

    bool Exists(Guid aggregateIdentifier, int version);

    Task<bool> ExistsAsync(Guid aggregateIdentifier, int version, CancellationToken cancellationToken = default);

    IReadOnlyCollection<DomainEventDataModel> Get(Guid aggregateIdentifier, int version);

    Task<IReadOnlyCollection<DomainEventDataModel>> GetAsync(Guid aggregateIdentifier, int version,
        CancellationToken cancellationToken = default);

    IEnumerable<Guid> GetDeleted();

    Task<IEnumerable<Guid>> GetDeletedAsync(CancellationToken cancellationToken = default);

    void Save(AggregateRoot aggregate, IEnumerable<DomainEventDataModel> events);

    Task SaveAsync(AggregateRoot aggregate, IEnumerable<DomainEventDataModel> events,
        CancellationToken cancellationToken = default);

    void Box(Guid aggregateIdentifier);

    Task BoxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);
}