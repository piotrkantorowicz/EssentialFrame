using System.Text;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;

public interface IEventSourcingAggregateRepository<TAggregate, TAggregateIdentifier, TType>
    where TAggregate : class, IEventSourcingAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    TAggregate Get(TAggregateIdentifier aggregateIdentifier);

    Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken);

    IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate);

    Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate,
        CancellationToken cancellationToken);

    IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate, int version);

    Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate, int version,
        CancellationToken cancellationToken);

    void Box(TAggregateIdentifier aggregateIdentifier);

    void Box(TAggregateIdentifier aggregateIdentifier, Encoding encoding);

    Task BoxAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken);

    Task BoxAsync(TAggregateIdentifier aggregateIdentifier, Encoding encoding, CancellationToken cancellationToken);

    TAggregate Unbox(TAggregateIdentifier aggregateIdentifier);

    TAggregate Unbox(TAggregateIdentifier aggregateIdentifier, Encoding encoding);

    Task<TAggregate> UnboxAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken);

    Task<TAggregate> UnboxAsync(TAggregateIdentifier aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken);
}