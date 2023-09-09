using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.Events.Core.Factories;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Services;

public sealed class
    AggregateRepository<TAggregate, TAggregateIdentifier> : IAggregateRepository<TAggregate, TAggregateIdentifier>
    where TAggregate : AggregateRoot<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    private readonly IAggregateMapper _aggregateMapper;
    private readonly IDomainEventMapper<TAggregateIdentifier> _domainEventMapper;
    private readonly IAggregateStore _aggregateStore;

    public AggregateRepository(IAggregateStore aggregateStore,
        IDomainEventMapper<TAggregateIdentifier> domainEventMapper, IAggregateMapper aggregateMapper)
    {
        _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore));
        _domainEventMapper = domainEventMapper ?? throw new ArgumentNullException(nameof(domainEventMapper));
        _aggregateMapper = aggregateMapper ?? throw new ArgumentNullException(nameof(aggregateMapper));
    }

    public TAggregate Get(TAggregateIdentifier aggregateIdentifier)
    {
        return Rehydrate(aggregateIdentifier);
    }

    public Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier,
        CancellationToken cancellationToken = default)
    {
        return RehydrateAsync(aggregateIdentifier, cancellationToken);
    }

    public IDomainEvent<TAggregateIdentifier>[] Save(TAggregate aggregate, int? version = null)
    {
        if (version != null && _aggregateStore.Exists(aggregate.AggregateIdentifier.Identifier, version.Value))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier.Identifier);
        }

        AggregateDataModel aggregateDataModel = _aggregateMapper.Map(aggregate);
        IDomainEvent<TAggregateIdentifier>[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = _domainEventMapper.Map(domainEvents);

        _aggregateStore.Save(aggregateDataModel, domainEventDataModels);

        return domainEvents;
    }

    public async Task<IDomainEvent<TAggregateIdentifier>[]> SaveAsync(TAggregate aggregate, int? version = null,
        CancellationToken cancellationToken = default)
    {
        if (version != null && await _aggregateStore.ExistsAsync(aggregate.AggregateIdentifier.Identifier,
                version.Value, cancellationToken))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier.Identifier);
        }

        AggregateDataModel aggregateDataModel = _aggregateMapper.Map(aggregate);
        IDomainEvent<TAggregateIdentifier>[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = _domainEventMapper.Map(domainEvents);

        await _aggregateStore.SaveAsync(aggregateDataModel, domainEventDataModels, cancellationToken);

        return domainEvents;
    }

    private TAggregate Rehydrate(TAggregateIdentifier aggregateIdentifier)
    {
        AggregateDataModel aggregateDataModel = _aggregateStore.Get(aggregateIdentifier.Identifier);

        if (aggregateDataModel is null)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), aggregateIdentifier.ToString());
        }

        IReadOnlyCollection<DomainEventDataModel> eventsData = _aggregateStore.Get(aggregateIdentifier.Identifier, -1);

        if (aggregateDataModel.IsDeleted)
        {
            throw new AggregateDeletedException(aggregateDataModel.AggregateIdentifier, typeof(TAggregate));
        }

        TAggregate aggregate = GenericAggregateFactory<TAggregate, TAggregateIdentifier>.CreateAggregate(
            TypedGuidIdentifier.New<TAggregateIdentifier>(aggregateDataModel.AggregateIdentifier) ??
            aggregateIdentifier);

        if (eventsData?.Any() != true)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), aggregateDataModel.AggregateIdentifier.ToString());
        }

        List<IDomainEvent<TAggregateIdentifier>> events = _domainEventMapper.Map(eventsData).ToList();

        aggregate.Rehydrate(events);

        return aggregate;
    }

    private async Task<TAggregate> RehydrateAsync(TAggregateIdentifier aggregateIdentifier,
        CancellationToken cancellationToken = default)
    {
        AggregateDataModel aggregateDataModel =
            await _aggregateStore.GetAsync(aggregateIdentifier.Identifier, cancellationToken);

        if (aggregateDataModel is null)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), aggregateIdentifier.ToString());
        }

        IReadOnlyCollection<DomainEventDataModel> eventsData =
            await _aggregateStore.GetAsync(aggregateIdentifier.Identifier, -1, cancellationToken);

        if (aggregateDataModel.IsDeleted)
        {
            throw new AggregateDeletedException(aggregateDataModel.AggregateIdentifier, typeof(TAggregate));
        }

        TAggregate aggregate = GenericAggregateFactory<TAggregate, TAggregateIdentifier>.CreateAggregate(
            TypedGuidIdentifier.New<TAggregateIdentifier>(aggregateDataModel.AggregateIdentifier) ??
            aggregateIdentifier);

        if (eventsData?.Any() != true)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), aggregateDataModel.AggregateIdentifier.ToString());
        }

        List<IDomainEvent<TAggregateIdentifier>> events = _domainEventMapper.Map(eventsData).ToList();

        aggregate.Rehydrate(events);

        return aggregate;
    }
}