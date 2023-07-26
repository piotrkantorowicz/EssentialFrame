using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;

public interface IAggregateOfflineStorage
{
    void Save(AggregateDataModel aggregate, IReadOnlyCollection<DomainEventDataModel> events);

    Task SaveAsync(AggregateDataModel aggregate, IReadOnlyCollection<DomainEventDataModel> events,
        CancellationToken cancellationToken = default);
}