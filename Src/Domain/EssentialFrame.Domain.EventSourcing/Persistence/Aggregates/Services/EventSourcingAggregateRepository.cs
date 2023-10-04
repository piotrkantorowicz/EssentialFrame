using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.Events.Services.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.EventSourcing.Core.Factories;
using EssentialFrame.Domain.EventSourcing.Exceptions;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services;

public class
    EventSourcingAggregateRepository<TAggregate, TAggregateIdentifier, TType> : IEventSourcingAggregateRepository<
        TAggregate, TAggregateIdentifier, TType>
    where TAggregate : class, IEventSourcingAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    private readonly IEventSourcingAggregateMapper<TAggregateIdentifier, TType> _eventSourcingAggregateMapper;
    private readonly IDomainEventMapper<TAggregateIdentifier, TType> _domainEventMapper;
    private readonly IEventSourcingAggregateStore _eventSourcingAggregateStore;
    private readonly IDomainEventsPublisher<TAggregateIdentifier, TType> _domainEventsPublisher;

    public EventSourcingAggregateRepository(IEventSourcingAggregateStore eventSourcingAggregateStore,
        IDomainEventMapper<TAggregateIdentifier, TType> domainEventMapper,
        IEventSourcingAggregateMapper<TAggregateIdentifier, TType> eventSourcingAggregateMapper,
        IDomainEventsPublisher<TAggregateIdentifier, TType> domainEventsPublisher)
    {
        _eventSourcingAggregateStore = eventSourcingAggregateStore ??
                                       throw new ArgumentNullException(nameof(eventSourcingAggregateStore));

        _domainEventMapper = domainEventMapper ?? throw new ArgumentNullException(nameof(domainEventMapper));

        _eventSourcingAggregateMapper = eventSourcingAggregateMapper ??
                                        throw new ArgumentNullException(nameof(eventSourcingAggregateMapper));

        _domainEventsPublisher =
            domainEventsPublisher ?? throw new ArgumentNullException(nameof(domainEventsPublisher));
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

    public IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate, int? version = null)
    {
        if (version != null && _eventSourcingAggregateStore.Exists(aggregate.AggregateIdentifier, version.Value))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = _eventSourcingAggregateMapper.Map(aggregate);
        IDomainEvent<TAggregateIdentifier, TType>[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = _domainEventMapper.Map(domainEvents);

        _eventSourcingAggregateStore.Save(eventSourcingAggregateDataModel, domainEventDataModels);

        foreach (IDomainEvent<TAggregateIdentifier, TType> domainEvent in domainEvents)
        {
            _domainEventsPublisher.Publish(domainEvent);
        }
        
        return domainEvents;
    }

    public async Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate, int? version = null,
        CancellationToken cancellationToken = default)
    {
        if (version != null && await _eventSourcingAggregateStore.ExistsAsync(aggregate.AggregateIdentifier,
                version.Value, cancellationToken))
        {
            throw new ConcurrencyException(aggregate.AggregateIdentifier);
        }

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = _eventSourcingAggregateMapper.Map(aggregate);
        IDomainEvent<TAggregateIdentifier, TType>[] domainEvents = aggregate.FlushUncommittedChanges();
        IEnumerable<DomainEventDataModel> domainEventDataModels = _domainEventMapper.Map(domainEvents);

        await _eventSourcingAggregateStore.SaveAsync(eventSourcingAggregateDataModel, domainEventDataModels,
            cancellationToken);

        foreach (IDomainEvent<TAggregateIdentifier, TType> domainEvent in domainEvents)
        {
            await _domainEventsPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        return domainEvents;
    }

    public void Box(TAggregateIdentifier aggregateIdentifier)
    {
        _eventSourcingAggregateStore.Box(aggregateIdentifier);
    }

    public async Task BoxAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        await _eventSourcingAggregateStore.BoxAsync(aggregateIdentifier, cancellationToken);
    }

    private TAggregate Rehydrate(TAggregateIdentifier aggregateIdentifier)
    {
        EventSourcingAggregateDataModel eventSourcingAggregateDataModel =
            _eventSourcingAggregateStore.Get(aggregateIdentifier);

        if (eventSourcingAggregateDataModel is null)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), aggregateIdentifier.ToString());
        }

        IReadOnlyCollection<DomainEventDataModel> eventsData =
            _eventSourcingAggregateStore.Get(aggregateIdentifier, -1);

        if (eventSourcingAggregateDataModel.IsDeleted)
        {
            throw new AggregateDeletedException(eventSourcingAggregateDataModel.AggregateIdentifier,
                typeof(TAggregate));
        }

        TAggregate aggregate =
            EventSourcingGenericAggregateFactory<TAggregate, TAggregateIdentifier, TType>.CreateAggregate(
                TypedIdentifierBase<TType>.New<TAggregateIdentifier>(
                    eventSourcingAggregateDataModel.AggregateIdentifier) ??
            aggregateIdentifier);

        if (eventsData?.Any() != true)
        {
            throw new AggregateNotFoundException(typeof(TAggregate),
                eventSourcingAggregateDataModel.AggregateIdentifier);
        }

        List<IDomainEvent<TAggregateIdentifier, TType>> events = _domainEventMapper.Map(eventsData).ToList();

        aggregate.Rehydrate(events);

        return aggregate;
    }

    private async Task<TAggregate> RehydrateAsync(TAggregateIdentifier aggregateIdentifier,
        CancellationToken cancellationToken = default)
    {
        EventSourcingAggregateDataModel eventSourcingAggregateDataModel =
            await _eventSourcingAggregateStore.GetAsync(aggregateIdentifier, cancellationToken);

        if (eventSourcingAggregateDataModel is null)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), aggregateIdentifier.ToString());
        }

        IReadOnlyCollection<DomainEventDataModel> eventsData =
            await _eventSourcingAggregateStore.GetAsync(aggregateIdentifier, -1, cancellationToken);

        if (eventSourcingAggregateDataModel.IsDeleted)
        {
            throw new AggregateDeletedException(eventSourcingAggregateDataModel.AggregateIdentifier,
                typeof(TAggregate));
        }

        TAggregate aggregate =
            EventSourcingGenericAggregateFactory<TAggregate, TAggregateIdentifier, TType>.CreateAggregate(
                TypedIdentifierBase<TType>.New<TAggregateIdentifier>(
                    eventSourcingAggregateDataModel.AggregateIdentifier) ??
            aggregateIdentifier);

        if (eventsData?.Any() != true)
        {
            throw new AggregateNotFoundException(typeof(TAggregate),
                eventSourcingAggregateDataModel.AggregateIdentifier);
        }

        List<IDomainEvent<TAggregateIdentifier, TType>> events = _domainEventMapper.Map(eventsData).ToList();

        aggregate.Rehydrate(events);

        return aggregate;
    }
}