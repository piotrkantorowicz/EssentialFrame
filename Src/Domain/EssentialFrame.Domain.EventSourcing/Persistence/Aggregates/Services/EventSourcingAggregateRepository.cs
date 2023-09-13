using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.EventSourcing.Core.Factories;
using EssentialFrame.Domain.EventSourcing.Exceptions;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services;

public sealed class
    EventSourcingAggregateRepository<TAggregate, TAggregateIdentifier> : IEventSourcingAggregateRepository<TAggregate,
        TAggregateIdentifier> where TAggregate : IEventSourcingAggregateRoot<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    private readonly IEventSourcingAggregateMapper<TAggregateIdentifier> _eventSourcingAggregateMapper;
    private readonly IDomainEventMapper<TAggregateIdentifier> _domainEventMapper;
    private readonly IEventSourcingAggregateStore _eventSourcingAggregateStore;

    public EventSourcingAggregateRepository(IEventSourcingAggregateStore eventSourcingAggregateStore,
        IDomainEventMapper<TAggregateIdentifier> domainEventMapper,
        IEventSourcingAggregateMapper<TAggregateIdentifier> eventSourcingAggregateMapper)
    {
        _eventSourcingAggregateStore = eventSourcingAggregateStore ??
                                       throw new ArgumentNullException(nameof(eventSourcingAggregateStore));
        _domainEventMapper = domainEventMapper ?? throw new ArgumentNullException(nameof(domainEventMapper));
        _eventSourcingAggregateMapper = eventSourcingAggregateMapper ??
                                        throw new ArgumentNullException(nameof(eventSourcingAggregateMapper));
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
        if (version != null && _eventSourcingAggregateStore.Exists(aggregate.AggregateIdentifier.Value, version.Value))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier.Value);
        }

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = _eventSourcingAggregateMapper.Map(aggregate);
        IDomainEvent<TAggregateIdentifier>[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = _domainEventMapper.Map(domainEvents);

        _eventSourcingAggregateStore.Save(eventSourcingAggregateDataModel, domainEventDataModels);

        return domainEvents;
    }

    public async Task<IDomainEvent<TAggregateIdentifier>[]> SaveAsync(TAggregate aggregate, int? version = null,
        CancellationToken cancellationToken = default)
    {
        if (version != null && await _eventSourcingAggregateStore.ExistsAsync(aggregate.AggregateIdentifier.Value,
                version.Value, cancellationToken))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier.Value);
        }

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = _eventSourcingAggregateMapper.Map(aggregate);
        IDomainEvent<TAggregateIdentifier>[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = _domainEventMapper.Map(domainEvents);

        await _eventSourcingAggregateStore.SaveAsync(eventSourcingAggregateDataModel, domainEventDataModels,
            cancellationToken);

        return domainEvents;
    }

    private TAggregate Rehydrate(TAggregateIdentifier aggregateIdentifier)
    {
        EventSourcingAggregateDataModel eventSourcingAggregateDataModel =
            _eventSourcingAggregateStore.Get(aggregateIdentifier.Value);

        if (eventSourcingAggregateDataModel is null)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), aggregateIdentifier.ToString());
        }

        IReadOnlyCollection<DomainEventDataModel> eventsData =
            _eventSourcingAggregateStore.Get(aggregateIdentifier.Value, -1);

        if (eventSourcingAggregateDataModel.IsDeleted)
        {
            throw new AggregateDeletedException(eventSourcingAggregateDataModel.AggregateIdentifier,
                typeof(TAggregate));
        }

        TAggregate aggregate = EventSourcingGenericAggregateFactory<TAggregate, TAggregateIdentifier>.CreateAggregate(
            TypedGuidIdentifier.New<TAggregateIdentifier>(eventSourcingAggregateDataModel.AggregateIdentifier) ??
            aggregateIdentifier);

        if (eventsData?.Any() != true)
        {
            throw new AggregateNotFoundException(typeof(TAggregate),
                eventSourcingAggregateDataModel.AggregateIdentifier.ToString());
        }

        List<IDomainEvent<TAggregateIdentifier>> events = _domainEventMapper.Map(eventsData).ToList();

        aggregate.Rehydrate(events);

        return aggregate;
    }

    private async Task<TAggregate> RehydrateAsync(TAggregateIdentifier aggregateIdentifier,
        CancellationToken cancellationToken = default)
    {
        EventSourcingAggregateDataModel eventSourcingAggregateDataModel =
            await _eventSourcingAggregateStore.GetAsync(aggregateIdentifier.Value, cancellationToken);

        if (eventSourcingAggregateDataModel is null)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), aggregateIdentifier.ToString());
        }

        IReadOnlyCollection<DomainEventDataModel> eventsData =
            await _eventSourcingAggregateStore.GetAsync(aggregateIdentifier.Value, -1, cancellationToken);

        if (eventSourcingAggregateDataModel.IsDeleted)
        {
            throw new AggregateDeletedException(eventSourcingAggregateDataModel.AggregateIdentifier,
                typeof(TAggregate));
        }

        TAggregate aggregate = EventSourcingGenericAggregateFactory<TAggregate, TAggregateIdentifier>.CreateAggregate(
            TypedGuidIdentifier.New<TAggregateIdentifier>(eventSourcingAggregateDataModel.AggregateIdentifier) ??
            aggregateIdentifier);

        if (eventsData?.Any() != true)
        {
            throw new AggregateNotFoundException(typeof(TAggregate),
                eventSourcingAggregateDataModel.AggregateIdentifier.ToString());
        }

        List<IDomainEvent<TAggregateIdentifier>> events = _domainEventMapper.Map(eventsData).ToList();

        aggregate.Rehydrate(events);

        return aggregate;
    }
}