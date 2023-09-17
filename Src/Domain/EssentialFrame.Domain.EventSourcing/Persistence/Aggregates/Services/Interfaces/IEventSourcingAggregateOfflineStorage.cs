using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Persistence.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;

public interface IEventSourcingAggregateOfflineStorage
{
    void Save(EventSourcingAggregateDataModel eventSourcingAggregate, IReadOnlyCollection<DomainEventDataModel> events);

    Task SaveAsync(EventSourcingAggregateDataModel eventSourcingAggregate,
        IReadOnlyCollection<DomainEventDataModel> events, CancellationToken cancellationToken = default);
}