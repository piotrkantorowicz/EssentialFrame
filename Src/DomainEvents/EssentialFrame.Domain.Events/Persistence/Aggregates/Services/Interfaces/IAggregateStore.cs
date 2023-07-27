using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;

public interface IAggregateStore
{
    bool Exists(Guid aggregateIdentifier);

    Task<bool> ExistsAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);

    bool Exists(Guid aggregateIdentifier, int version);

    Task<bool> ExistsAsync(Guid aggregateIdentifier, int version, CancellationToken cancellationToken = default);

    AggregateDataModel Get(Guid aggregateIdentifier);

    Task<AggregateDataModel> GetAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);

    IReadOnlyCollection<DomainEventDataModel> Get(Guid aggregateIdentifier, int version);

    Task<IReadOnlyCollection<DomainEventDataModel>> GetAsync(Guid aggregateIdentifier, int version,
        CancellationToken cancellationToken = default);

    IEnumerable<Guid> GetDeleted();

    Task<IEnumerable<Guid>> GetDeletedAsync(CancellationToken cancellationToken = default);

    void Save(AggregateDataModel aggregate, IEnumerable<DomainEventDataModel> events);

    Task SaveAsync(AggregateDataModel aggregate, IEnumerable<DomainEventDataModel> events,
        CancellationToken cancellationToken = default);

    void Box(Guid aggregateIdentifier);

    Task BoxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);
}