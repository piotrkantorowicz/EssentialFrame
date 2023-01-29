using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.Events.Snapshots.Interfaces;

public interface ISnapshotRepository
{
    T Get<T>(Guid aggregateId) where T : AggregateRoot;

    Task<T> GetAsync<T>(Guid aggregateId, CancellationToken cancellationToken = default) where T : AggregateRoot;

    IDomainEvent[] Save<T>(T aggregate, int? version = null, int? timeout = null) where T : AggregateRoot;

    Task<IDomainEvent[]> SaveAsync<T>(T aggregate, int? version = null, int? timeout = null,
        CancellationToken cancellationToken = default) where T : AggregateRoot;

    void Box<T>(T aggregate) where T : AggregateRoot;

    Task BoxAsync<T>(T aggregate, CancellationToken cancellationToken = default) where T : AggregateRoot;

    T Unbox<T>(Guid aggregateId) where T : AggregateRoot;

    Task<T> UnboxAsync<T>(Guid aggregateId, CancellationToken cancellationToken = default) where T : AggregateRoot;

    void Ping();

    Task PingAsync(CancellationToken cancellationToken = default);
}