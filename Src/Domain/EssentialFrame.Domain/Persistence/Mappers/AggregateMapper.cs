using EssentialFrame.Domain.Core.Aggregates;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Persistence.Mappers;

internal sealed class
    AggregateMapper<TAggregateRoot, TAggregateIdentifier, TType> : IAggregateMapper<TAggregateRoot, TAggregateIdentifier
        , TType> where TAggregateRoot : class, IAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    public AggregateDataModel Map(TAggregateRoot aggregate)

    {
        return new AggregateDataModel
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            TenantIdentifier = aggregate.TenantIdentifier,
            State = aggregate
        };
    }

    public AggregateDataModel Map(TAggregateRoot aggregate, ISerializer serializer)
    {
        return new AggregateDataModel
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            TenantIdentifier = aggregate.TenantIdentifier,
            State = serializer.Serialize(aggregate)
        };
    }

    public TAggregateRoot Map(AggregateDataModel aggregateDataModel)
    {
        return (TAggregateRoot)aggregateDataModel.State;
    }

    public TAggregateRoot Map(AggregateDataModel aggregateDataModel, ISerializer serializer)
    {
        return serializer.Deserialize<TAggregateRoot>(aggregateDataModel.State.ToString(), typeof(TAggregateRoot));
    }
}