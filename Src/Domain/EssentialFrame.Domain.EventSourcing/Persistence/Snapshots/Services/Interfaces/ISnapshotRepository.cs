using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services.Interfaces;

public interface ISnapshotRepository<TAggregate, TAggregateIdentifier, TType>
    where TAggregate : class, IEventSourcingAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    TAggregate Get(TAggregateIdentifier aggregateIdentifier);

    Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken = default);

    IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate, int? version = null, int? timeout = null);

    Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate, int? version = null,
        int? timeout = null,
        CancellationToken cancellationToken = default);

    void Box(TAggregate aggregate, bool useSerializer = false);

    Task BoxAsync(TAggregate aggregate, bool useSerializer = false, CancellationToken cancellationToken = default);

    TAggregate Unbox(TAggregateIdentifier aggregateIdentifier, bool useSerializer = false);

    Task<TAggregate> UnboxAsync(TAggregateIdentifier aggregateIdentifier, bool useSerializer = false,
        CancellationToken cancellationToken = default);
}