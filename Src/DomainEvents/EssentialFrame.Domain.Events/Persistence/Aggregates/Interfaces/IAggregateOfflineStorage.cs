using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Persistence.DomainEvents;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Interfaces;

public interface IAggregateOfflineStorage
{
    void Save(AggregateRoot aggregate, IReadOnlyCollection<DomainEventDataModel> events);

    Task SaveAsync(AggregateRoot aggregate, IReadOnlyCollection<DomainEventDataModel> events,
        CancellationToken cancellationToken = default);
}