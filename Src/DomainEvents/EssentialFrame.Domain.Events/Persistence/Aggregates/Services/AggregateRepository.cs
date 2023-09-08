using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.Events.Core.Factories;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Services;

public sealed class AggregateRepository<TAggregate, TAggregateId> : IAggregateRepository<TAggregate, TAggregateId>
    where TAggregate : AggregateRoot<TAggregateId> where TAggregateId : TypedGuidIdentifier
{
    private readonly IAggregateMapper _aggregateMapper;
    private readonly IDomainEventMapper _domainEventMapper;
    private readonly IAggregateStore _aggregateStore;

    public AggregateRepository(IAggregateStore aggregateStore, IDomainEventMapper domainEventMapper,
        IAggregateMapper aggregateMapper)
    {
        _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore));
        _domainEventMapper = domainEventMapper ?? throw new ArgumentNullException(nameof(domainEventMapper));
        _aggregateMapper = aggregateMapper ?? throw new ArgumentNullException(nameof(aggregateMapper));
    }

    public TAggregate Get(Guid aggregate)
    {
        return Rehydrate(aggregate);
    }

    public Task<TAggregate> GetAsync(Guid aggregate, CancellationToken cancellationToken = default)
    {
        return RehydrateAsync(aggregate, cancellationToken);
    }

    public IDomainEvent[] Save(TAggregate aggregate, int? version = null)
    {
        if (version != null && _aggregateStore.Exists(aggregate.AggregateIdentifier, version.Value))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        AggregateDataModel aggregateDataModel = _aggregateMapper.Map(aggregate);
        IDomainEvent[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = _domainEventMapper.Map(domainEvents);

        _aggregateStore.Save(aggregateDataModel, domainEventDataModels);

        return domainEvents;
    }

    public async Task<IDomainEvent[]> SaveAsync(TAggregate aggregate, int? version = null,
        CancellationToken cancellationToken = default)
    {
        if (version != null &&
            await _aggregateStore.ExistsAsync(aggregate.AggregateIdentifier, version.Value, cancellationToken))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        AggregateDataModel aggregateDataModel = _aggregateMapper.Map(aggregate);
        IDomainEvent[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = _domainEventMapper.Map(domainEvents);

        await _aggregateStore.SaveAsync(aggregateDataModel, domainEventDataModels, cancellationToken);

        return domainEvents;
    }

    private TAggregate Rehydrate(Guid id)
    {
        AggregateDataModel aggregateDataModel = _aggregateStore.Get(id);
        IReadOnlyCollection<DomainEventDataModel> eventsData = _aggregateStore.Get(id, -1);

        if (aggregateDataModel?.IsDeleted == true)
        {
            throw new AggregateDeletedException(aggregateDataModel.AggregateIdentifier, typeof(TAggregate));
        }

        TAggregate aggregate =
            GenericAggregateFactory<TAggregate, TAggregateId>.CreateAggregate(aggregateDataModel?.AggregateIdentifier ??
                                                                              id);

        if (eventsData?.Any() != true)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), aggregateDataModel?.AggregateIdentifier ?? id);
        }

        List<IDomainEvent> events = _domainEventMapper.Map(eventsData).ToList();

        aggregate.Rehydrate(events);

        return aggregate;
    }

    private async Task<TAggregate> RehydrateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        AggregateDataModel aggregateDataModel = await _aggregateStore.GetAsync(id, cancellationToken);

        IReadOnlyCollection<DomainEventDataModel>
            eventsData = await _aggregateStore.GetAsync(id, -1, cancellationToken);

        if (aggregateDataModel?.IsDeleted == true)
        {
            throw new AggregateDeletedException(aggregateDataModel.AggregateIdentifier, typeof(TAggregate));
        }

        TAggregate aggregate =
            GenericAggregateFactory<TAggregate, TAggregateId>.CreateAggregate(aggregateDataModel?.AggregateIdentifier ??
                                                                              id);

        if (eventsData?.Any() != true)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), aggregateDataModel?.AggregateIdentifier ?? id);
        }

        List<IDomainEvent> events = _domainEventMapper.Map(eventsData).ToList();

        aggregate.Rehydrate(events);

        return aggregate;
    }
}