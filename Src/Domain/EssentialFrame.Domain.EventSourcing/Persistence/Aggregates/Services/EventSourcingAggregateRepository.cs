using System.Text;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.Events.Services.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.EventSourcing.Core.Factories;
using EssentialFrame.Domain.EventSourcing.Exceptions;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services;

internal sealed class
    EventSourcingAggregateRepository<TAggregate, TAggregateIdentifier, TType> : IEventSourcingAggregateRepository<
        TAggregate, TAggregateIdentifier, TType>
    where TAggregate : class, IEventSourcingAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    private readonly IEventSourcingAggregateMapper<TAggregate, TAggregateIdentifier, TType>
        _eventSourcingAggregateMapper;
    private readonly IDomainEventMapper<TAggregateIdentifier, TType> _domainEventMapper;
    private readonly IEventSourcingAggregateStore _eventSourcingAggregateStore;
    private readonly IDomainEventsPublisher<TAggregateIdentifier, TType> _domainEventsPublisher;

    public EventSourcingAggregateRepository(IEventSourcingAggregateStore eventSourcingAggregateStore,
        IDomainEventMapper<TAggregateIdentifier, TType> domainEventMapper,
        IEventSourcingAggregateMapper<TAggregate, TAggregateIdentifier, TType> eventSourcingAggregateMapper,
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
        return RehydrateInternal(aggregateIdentifier);
    }

    public Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken)
    {
        return RehydrateInternalAsync(aggregateIdentifier, cancellationToken);
    }

    public IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate)
    {
        return SaveInternal(aggregate, null);
    }

    public IDomainEvent<TAggregateIdentifier, TType>[] Save(TAggregate aggregate, int version)
    {
        return SaveInternal(aggregate, version);
    }

    public async Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate,
        CancellationToken cancellationToken)
    {
        return await SaveInternalAsync(aggregate, null, cancellationToken);
    }

    public async Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveAsync(TAggregate aggregate, int version,
        CancellationToken cancellationToken)
    {
        return await SaveInternalAsync(aggregate, version, cancellationToken);
    }

    public void Box(TAggregateIdentifier aggregateIdentifier)
    {
        _eventSourcingAggregateStore.Box(aggregateIdentifier, Encoding.Unicode);
    }

    public void Box(TAggregateIdentifier aggregateIdentifier, Encoding encoding)
    {
        _eventSourcingAggregateStore.Box(aggregateIdentifier, encoding);
    }

    public async Task BoxAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken)
    {
        await _eventSourcingAggregateStore.BoxAsync(aggregateIdentifier, Encoding.Unicode, cancellationToken);
    }

    public async Task BoxAsync(TAggregateIdentifier aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken)
    {
        await _eventSourcingAggregateStore.BoxAsync(aggregateIdentifier, encoding, cancellationToken);
    }

    public TAggregate Unbox(TAggregateIdentifier aggregateIdentifier)
    {
        EventSourcingAggregateWithEventsModel eventSourcingAggregateWithEventsModel =
            _eventSourcingAggregateStore.Unbox(aggregateIdentifier, Encoding.Unicode);

        return RehydrateInternal(aggregateIdentifier, eventSourcingAggregateWithEventsModel.DomainEventDataModels);
    }

    public TAggregate Unbox(TAggregateIdentifier aggregateIdentifier, Encoding encoding)
    {
        EventSourcingAggregateWithEventsModel eventSourcingAggregateWithEventsModel =
            _eventSourcingAggregateStore.Unbox(aggregateIdentifier, encoding);

        return RehydrateInternal(aggregateIdentifier, eventSourcingAggregateWithEventsModel.DomainEventDataModels);
    }

    public async Task<TAggregate> UnboxAsync(TAggregateIdentifier aggregateIdentifier,
        CancellationToken cancellationToken)
    {
        EventSourcingAggregateWithEventsModel eventSourcingAggregateWithEventsModel =
            await _eventSourcingAggregateStore.UnboxAsync(aggregateIdentifier, Encoding.Unicode, cancellationToken);

        return RehydrateInternal(aggregateIdentifier, eventSourcingAggregateWithEventsModel.DomainEventDataModels);
    }

    public async Task<TAggregate> UnboxAsync(TAggregateIdentifier aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken)
    {
        EventSourcingAggregateWithEventsModel eventSourcingAggregateWithEventsModel =
            await _eventSourcingAggregateStore.UnboxAsync(aggregateIdentifier, encoding, cancellationToken);

        return RehydrateInternal(aggregateIdentifier, eventSourcingAggregateWithEventsModel.DomainEventDataModels);
    }

    private TAggregate RehydrateInternal(TAggregateIdentifier aggregateIdentifier)
    {
        EventSourcingAggregateDataModel eventSourcingAggregateDataModel =
            _eventSourcingAggregateStore.Get(aggregateIdentifier);

        if (eventSourcingAggregateDataModel is null)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), aggregateIdentifier.ToString());
        }

        IReadOnlyCollection<DomainEventDataModel> eventDataModels =
            _eventSourcingAggregateStore.Get(aggregateIdentifier, -1);

        return RehydrateInternal(aggregateIdentifier, eventDataModels);
    }

    private async Task<TAggregate> RehydrateInternalAsync(TAggregateIdentifier aggregateIdentifier,
        CancellationToken cancellationToken)
    {
        EventSourcingAggregateDataModel eventSourcingAggregateDataModel =
            await _eventSourcingAggregateStore.GetAsync(aggregateIdentifier, cancellationToken);

        if (eventSourcingAggregateDataModel is null)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), aggregateIdentifier.ToString());
        }

        IReadOnlyCollection<DomainEventDataModel> eventDataModels =
            await _eventSourcingAggregateStore.GetAsync(aggregateIdentifier, -1, cancellationToken);

        return RehydrateInternal(aggregateIdentifier, eventDataModels);
    }

    private TAggregate RehydrateInternal(TAggregateIdentifier aggregateIdentifier,
        IReadOnlyCollection<DomainEventDataModel> domainEventDataModels)
    {
        TAggregate aggregate =
            EventSourcingGenericAggregateFactory<TAggregate, TAggregateIdentifier, TType>.CreateAggregate(
                TypedIdentifierBase<TType>.New<TAggregateIdentifier>(aggregateIdentifier));

        if (domainEventDataModels?.Any() != true)
        {
            throw new AggregateNotFoundException(typeof(TAggregate), aggregateIdentifier);
        }

        List<IDomainEvent<TAggregateIdentifier, TType>> events = _domainEventMapper.Map(domainEventDataModels).ToList();

        aggregate.Rehydrate(events);

        return aggregate;
    }

    private IDomainEvent<TAggregateIdentifier, TType>[] SaveInternal(TAggregate aggregate, int? version)
    {
        if (version.HasValue && _eventSourcingAggregateStore.Exists(aggregate.AggregateIdentifier, version.Value))
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

    private async Task<IDomainEvent<TAggregateIdentifier, TType>[]> SaveInternalAsync(TAggregate aggregate,
        int? version,
        CancellationToken cancellationToken)
    {
        if (version.HasValue && await _eventSourcingAggregateStore.ExistsAsync(aggregate.AggregateIdentifier,
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
}