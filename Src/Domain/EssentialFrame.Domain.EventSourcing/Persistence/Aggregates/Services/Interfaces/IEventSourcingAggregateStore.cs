using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Persistence.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;

public interface IEventSourcingAggregateStore
{
    bool Exists(string aggregateIdentifier);

    Task<bool> ExistsAsync(string aggregateIdentifier, CancellationToken cancellationToken = default);

    bool Exists(string aggregateIdentifier, int version);

    Task<bool> ExistsAsync(string aggregateIdentifier, int version, CancellationToken cancellationToken = default);

    EventSourcingAggregateDataModel Get(string aggregateIdentifier);

    Task<EventSourcingAggregateDataModel> GetAsync(string aggregateIdentifier,
        CancellationToken cancellationToken = default);

    IReadOnlyCollection<DomainEventDataModel> Get(string aggregateIdentifier, int version);

    Task<IReadOnlyCollection<DomainEventDataModel>> GetAsync(string aggregateIdentifier, int version,
        CancellationToken cancellationToken = default);

    IEnumerable<string> GetExpired();

    Task<IEnumerable<string>> GetExpiredAsync(CancellationToken cancellationToken = default);

    void Save(EventSourcingAggregateDataModel eventSourcingAggregate, IEnumerable<DomainEventDataModel> events);

    Task SaveAsync(EventSourcingAggregateDataModel eventSourcingAggregate, IEnumerable<DomainEventDataModel> events,
        CancellationToken cancellationToken = default);

    void Box(string aggregateIdentifier);

    Task BoxAsync(string aggregateIdentifier, CancellationToken cancellationToken = default);
}