using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.Events.Persistence.DomainEvents.Interfaces;

public interface IAggregateOfflineStorage
{
    void Save(AggregateRoot aggregate, IReadOnlyCollection<IDomainEvent> events);

    Task SaveAsync(AggregateRoot aggregate, IReadOnlyCollection<IDomainEvent> events,
        CancellationToken cancellationToken = default);
}