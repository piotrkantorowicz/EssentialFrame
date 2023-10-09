using System.Text;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Persistence.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;

public interface IEventSourcingAggregateStore
{
    bool Exists(string aggregateIdentifier);

    Task<bool> ExistsAsync(string aggregateIdentifier, CancellationToken cancellationToken);

    bool Exists(string aggregateIdentifier, int version);

    Task<bool> ExistsAsync(string aggregateIdentifier, int version, CancellationToken cancellationToken);

    EventSourcingAggregateDataModel Get(string aggregateIdentifier);

    Task<EventSourcingAggregateDataModel> GetAsync(string aggregateIdentifier, CancellationToken cancellationToken);

    IReadOnlyCollection<DomainEventDataModel> Get(string aggregateIdentifier, int version);

    Task<IReadOnlyCollection<DomainEventDataModel>> GetAsync(string aggregateIdentifier, int version,
        CancellationToken cancellationToken);

    IEnumerable<string> GetExpired();

    Task<IEnumerable<string>> GetExpiredAsync(CancellationToken cancellationToken);

    void Save(EventSourcingAggregateDataModel eventSourcingAggregate, IEnumerable<DomainEventDataModel> events);

    Task SaveAsync(EventSourcingAggregateDataModel eventSourcingAggregate, IEnumerable<DomainEventDataModel> events,
        CancellationToken cancellationToken);

    void Box(string aggregateIdentifier);

    void Box(string aggregateIdentifier, Encoding encoding);

    Task BoxAsync(string aggregateIdentifier, CancellationToken cancellationToken);

    Task BoxAsync(string aggregateIdentifier, Encoding encoding, CancellationToken cancellationToken);
}