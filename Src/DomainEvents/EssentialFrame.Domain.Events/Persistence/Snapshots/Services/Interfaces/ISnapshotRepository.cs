using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.Events.Persistence.Snapshots.Services.Interfaces;

public interface ISnapshotRepository
{
    T Get<T>(Guid aggregateId) where T : AggregateRoot;

    Task<T> GetAsync<T>(Guid aggregateId, CancellationToken cancellationToken = default) where T : AggregateRoot;

    IDomainEvent[] Save<T>(T aggregate, int? version = null, int? timeout = null) where T : AggregateRoot;

    Task<IDomainEvent[]> SaveAsync<T>(T aggregate, int? version = null, int? timeout = null,
        CancellationToken cancellationToken = default) where T : AggregateRoot;

    void Box<T>(T aggregate, bool useSerializer = false) where T : AggregateRoot;

    Task BoxAsync<T>(T aggregate, bool useSerializer = false, CancellationToken cancellationToken = default)
        where T : AggregateRoot;

    T Unbox<T>(Guid aggregateId, bool useSerializer = false) where T : AggregateRoot;

    Task<T> UnboxAsync<T>(Guid aggregateId, bool useSerializer = false, CancellationToken cancellationToken = default)
        where T : AggregateRoot;
}