using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Events.Persistence.Snapshots.Services.Interfaces;

public interface ISnapshotRepository<TAggregate, TAggregateId> where TAggregate : AggregateRoot<TAggregateId>
    where TAggregateId : TypedGuidIdentifier
{
    TAggregate Get(Guid aggregateId);

    Task<TAggregate> GetAsync(Guid aggregateId, CancellationToken cancellationToken = default);

    IDomainEvent[] Save(TAggregate aggregate, int? version = null, int? timeout = null);

    Task<IDomainEvent[]> SaveAsync(TAggregate aggregate, int? version = null, int? timeout = null,
        CancellationToken cancellationToken = default);

    void Box(TAggregate aggregate, bool useSerializer = false);

    Task BoxAsync(TAggregate aggregate, bool useSerializer = false, CancellationToken cancellationToken = default);

    TAggregate Unbox(Guid aggregateId, bool useSerializer = false);

    Task<TAggregate> UnboxAsync(Guid aggregateId, bool useSerializer = false,
        CancellationToken cancellationToken = default);
}