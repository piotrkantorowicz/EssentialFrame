using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services.Interfaces;

public interface ISnapshotRepository<TAggregate, TAggregateIdentifier, TType>
    where TAggregate : class, IEventSourcingAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    TAggregate Get(TAggregateIdentifier aggregateIdentifier, bool useSerializer);

    Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier, bool useSerializer,
        CancellationToken cancellationToken);

    IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate);

    IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate, int version);

    IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate, int version, int timeout);

    Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate,
        CancellationToken cancellationToken);

    Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate, int version,
        CancellationToken cancellationToken);

    Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate, int version, int timeout,
        CancellationToken cancellationToken);

    void Box(TAggregate aggregate, bool useSerializer);

    Task BoxAsync(TAggregate aggregate, bool useSerializer, CancellationToken cancellationToken);

    TAggregate Unbox(TAggregateIdentifier aggregateIdentifier, bool useSerializer);

    Task<TAggregate> UnboxAsync(TAggregateIdentifier aggregateIdentifier, bool useSerializer,
        CancellationToken cancellationToken);
    
}