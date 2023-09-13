using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;

public interface IEventSourcingAggregateStore
{
    bool Exists(Guid aggregateIdentifier);

    Task<bool> ExistsAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);

    bool Exists(Guid aggregateIdentifier, int version);

    Task<bool> ExistsAsync(Guid aggregateIdentifier, int version, CancellationToken cancellationToken = default);

    EventSourcingAggregateDataModel Get(Guid aggregateIdentifier);

    Task<EventSourcingAggregateDataModel> GetAsync(Guid aggregateIdentifier,
        CancellationToken cancellationToken = default);

    IReadOnlyCollection<DomainEventDataModel> Get(Guid aggregateIdentifier, int version);

    Task<IReadOnlyCollection<DomainEventDataModel>> GetAsync(Guid aggregateIdentifier, int version,
        CancellationToken cancellationToken = default);

    IEnumerable<Guid> GetDeleted();

    Task<IEnumerable<Guid>> GetDeletedAsync(CancellationToken cancellationToken = default);

    void Save(EventSourcingAggregateDataModel eventSourcingAggregate, IEnumerable<DomainEventDataModel> events);

    Task SaveAsync(EventSourcingAggregateDataModel eventSourcingAggregate, IEnumerable<DomainEventDataModel> events,
        CancellationToken cancellationToken = default);

    void Box(Guid aggregateIdentifier);

    Task BoxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);
}