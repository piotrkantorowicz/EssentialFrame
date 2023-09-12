using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;

public interface IAggregateRepository<TAggregate, TAggregateIdentifier>
    where TAggregate : IAggregateRoot<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    TAggregate Get(TAggregateIdentifier aggregateIdentifier);

    Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken = default);

    IDomainEvent<TAggregateIdentifier>[] Save(TAggregate aggregate, int? version = null);

    Task<IDomainEvent<TAggregateIdentifier>[]> SaveAsync(TAggregate aggregate, int? version = null,
        CancellationToken cancellationToken = default);
}