using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;

public interface IAggregateRepository<TAggregate, TAggregateId> where TAggregate : AggregateRoot<TAggregateId>
    where TAggregateId : TypedGuidIdentifier
{
    TAggregate Get(Guid aggregate);

    Task<TAggregate> GetAsync(Guid aggregate, CancellationToken cancellationToken = default);

    IDomainEvent[] Save(TAggregate aggregate, int? version = null);

    Task<IDomainEvent[]> SaveAsync(TAggregate aggregate, int? version = null,
        CancellationToken cancellationToken = default);
}