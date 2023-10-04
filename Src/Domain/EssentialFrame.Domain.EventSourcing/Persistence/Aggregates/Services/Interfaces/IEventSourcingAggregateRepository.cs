using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;

public interface IEventSourcingAggregateRepository<TAggregate, TAggregateIdentifier, TType>
    where TAggregate : class, IEventSourcingAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    TAggregate Get(TAggregateIdentifier aggregateIdentifier);

    Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken = default);

    IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate, int? version = null);

    Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate, int? version = null,
        CancellationToken cancellationToken = default);

    void Box(TAggregateIdentifier aggregateIdentifier);

    Task BoxAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken = default);
}