using System.Text;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Persistence.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;

internal interface IEventSourcingAggregateOfflineStorage
{
    void Save(EventSourcingAggregateDataModel eventSourcingAggregate, IReadOnlyCollection<DomainEventDataModel> events,
        Encoding encoding);

    Task SaveAsync(EventSourcingAggregateDataModel eventSourcingAggregate,
        IReadOnlyCollection<DomainEventDataModel> events, Encoding encoding, CancellationToken cancellationToken);
}