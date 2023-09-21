using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;

public interface IEventSourcingAggregateRepository<TAggregate, TAggregateIdentifier>
    where TAggregate : class, IEventSourcingAggregateRoot<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    TAggregate Get(TAggregateIdentifier aggregateIdentifier);

    Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken = default);

    IDomainEvent<TAggregateIdentifier>[] Save(TAggregate aggregate, int? version = null);

    Task<IDomainEvent<TAggregateIdentifier>[]> SaveAsync(TAggregate aggregate, int? version = null,
        CancellationToken cancellationToken = default);

    void Box(TAggregateIdentifier aggregateIdentifier);

    Task BoxAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken = default);
}