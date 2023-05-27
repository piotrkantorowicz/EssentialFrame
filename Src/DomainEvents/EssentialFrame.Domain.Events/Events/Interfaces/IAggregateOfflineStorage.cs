using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.Events.Events.Interfaces;

public interface IAggregateOfflineStorage
{
    void Save(AggregateRoot aggregate, IReadOnlyCollection<IDomainEvent> events);

    Task SaveAsync(AggregateRoot aggregate, IReadOnlyCollection<IDomainEvent> events,
        CancellationToken cancellationToken = default);
}