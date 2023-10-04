using EssentialFrame.Domain.Core.Aggregates;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.Events.Services.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Domain.Persistence.Services.Interfaces;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Persistence.Services;

public class
    AggregateRepository<TAggregate, TAggregateIdentifier, TType> : IAggregateRepository<TAggregate, TAggregateIdentifier
        , TType> where TAggregate : class, IAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    private readonly IAggregateStore _aggregateStore;
    private readonly IAggregateMapper<TAggregateIdentifier, TType> _aggregateMapper;
    private readonly IDomainEventsPublisher<TAggregateIdentifier, TType> _domainEventsPublisher;

    protected AggregateRepository(IAggregateStore aggregateStore,
        IAggregateMapper<TAggregateIdentifier, TType> aggregateMapper,
        IDomainEventsPublisher<TAggregateIdentifier, TType> domainEventsPublisher)
    {
        _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore));
        _aggregateMapper = aggregateMapper ?? throw new ArgumentNullException(nameof(aggregateMapper));
        _domainEventsPublisher =
            domainEventsPublisher ?? throw new ArgumentNullException(nameof(domainEventsPublisher));
        
    }

    public TAggregate Get(TAggregateIdentifier aggregateIdentifier)
    {
        AggregateDataModel aggregateDataModel = _aggregateStore.Get(aggregateIdentifier);

        if (aggregateDataModel?.State is null)
        {
            throw new AggregateHasNotFoundException(typeof(TAggregate), aggregateIdentifier.ToString());
        }

        if (aggregateDataModel.State is TAggregate aggregate)
        {
            return aggregate;
        }

        throw new InvalidAggregateTypeException(aggregateDataModel.State.GetType(), typeof(TAggregate),
            aggregateIdentifier.ToString());
    }

    public TAggregate Get(TAggregateIdentifier aggregateIdentifier, ISerializer serializer)
    {
        AggregateDataModel aggregateDataModel = _aggregateStore.Get(aggregateIdentifier);

        if (aggregateDataModel.State is not string stateString)
        {
            return Get(aggregateIdentifier);
        }

        TAggregate aggregate = serializer.Deserialize<TAggregate>(stateString, typeof(TAggregate));

        return aggregate;
    }

    public async Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier,
        CancellationToken cancellationToken = default)
    {
        AggregateDataModel aggregateDataModel = await _aggregateStore.GetAsync(aggregateIdentifier, cancellationToken);

        if (aggregateDataModel?.State is null)
        {
            throw new AggregateHasNotFoundException(typeof(TAggregate), aggregateIdentifier.ToString());
        }

        if (aggregateDataModel.State is TAggregate aggregate)
        {
            return aggregate;
        }

        throw new InvalidAggregateTypeException(aggregateDataModel.State.GetType(), typeof(TAggregate),
            aggregateIdentifier.ToString());
    }

    public async Task<TAggregate> GetAsync(TAggregateIdentifier aggregateIdentifier, ISerializer serializer,
        CancellationToken cancellationToken = default)
    {
        AggregateDataModel aggregateDataModel = await _aggregateStore.GetAsync(aggregateIdentifier, cancellationToken);

        if (aggregateDataModel.State is not string stateString)
        {
            return await GetAsync(aggregateIdentifier, cancellationToken);
        }

        TAggregate aggregate = serializer.Deserialize<TAggregate>(stateString, typeof(TAggregate));

        return aggregate;
    }

    public void Save(TAggregate aggregate)
    {
        AggregateDataModel aggregateDataModel = _aggregateMapper.Map(aggregate);

        _aggregateStore.Save(aggregateDataModel);

        IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> domainEvents = aggregate.GetUncommittedChanges();

        foreach (IDomainEvent<TAggregateIdentifier, TType> domainEvent in domainEvents)
        {
            _domainEventsPublisher.Publish(domainEvent);
        }

        aggregate.ClearDomainEvents();
    }

    public async Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        AggregateDataModel aggregateDataModel = _aggregateMapper.Map(aggregate);

        await _aggregateStore.SaveAsync(aggregateDataModel, cancellationToken);

        IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> domainEvents = aggregate.GetUncommittedChanges();

        foreach (IDomainEvent<TAggregateIdentifier, TType> domainEvent in domainEvents)
        {
            await _domainEventsPublisher.PublishAsync(domainEvent, cancellationToken);
        }

        aggregate.ClearDomainEvents();
    }

    public void Box(TAggregateIdentifier aggregateIdentifier)
    {
        _aggregateStore.Box(aggregateIdentifier);
    }

    public async Task BoxAsync(TAggregateIdentifier aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        await _aggregateStore.BoxAsync(aggregateIdentifier, cancellationToken);
    }
}