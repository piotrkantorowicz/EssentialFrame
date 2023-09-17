using EssentialFrame.Domain.Core.Aggregates;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Domain.Persistence.Services.Interfaces;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Persistence.Services;

internal sealed class
    AggregateRepository<TAggregate, TAggregateIdentifier> : IAggregateRepository<TAggregate, TAggregateIdentifier>
    where TAggregate : class, IAggregateRoot<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    private readonly IAggregateStore _aggregateStore;
    private readonly IAggregateMapper<TAggregateIdentifier> _aggregateMapper;

    public AggregateRepository(IAggregateStore aggregateStore, IAggregateMapper<TAggregateIdentifier> aggregateMapper)
    {
        _aggregateStore = aggregateStore ?? throw new ArgumentNullException(nameof(aggregateStore));
        _aggregateMapper = aggregateMapper ?? throw new ArgumentNullException(nameof(aggregateMapper));
    }

    public TAggregate Get(TAggregateIdentifier aggregateIdentifier)
    {
        AggregateDataModel aggregateDataModel = _aggregateStore.Get(aggregateIdentifier.Value);

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
        AggregateDataModel aggregateDataModel = _aggregateStore.Get(aggregateIdentifier.Value);

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
        AggregateDataModel aggregateDataModel =
            await _aggregateStore.GetAsync(aggregateIdentifier.Value, cancellationToken);

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
        AggregateDataModel aggregateDataModel =
            await _aggregateStore.GetAsync(aggregateIdentifier.Value, cancellationToken);

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
    }

    public async Task SaveAsync(TAggregate aggregate, CancellationToken cancellationToken = default)
    {
        AggregateDataModel aggregateDataModel = _aggregateMapper.Map(aggregate);

        await _aggregateStore.SaveAsync(aggregateDataModel, cancellationToken);
    }
}