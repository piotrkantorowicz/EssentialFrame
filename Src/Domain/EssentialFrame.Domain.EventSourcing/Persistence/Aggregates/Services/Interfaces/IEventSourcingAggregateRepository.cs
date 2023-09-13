using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;

public interface IEventSourcingAggregateRepository<TAggregate, TAggregateIdentifier>
    where TAggregate : IEventSourcingAggregateRoot<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    TAggregate Get(TAggregateIdentifier aggregateIdentifier);

    Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken = default);

    IDomainEvent<TAggregateIdentifier>[] Save(TAggregate aggregate, int? version = null);

    Task<IDomainEvent<TAggregateIdentifier>[]> SaveAsync(TAggregate aggregate, int? version = null,
        CancellationToken cancellationToken = default);
}