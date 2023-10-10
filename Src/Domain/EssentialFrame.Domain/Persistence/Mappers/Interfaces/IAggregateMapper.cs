using EssentialFrame.Domain.Core.Aggregates;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Persistence.Mappers.Interfaces;

public interface IAggregateMapper<TAggregateRoot, TAggregateIdentifier, TType>
    where TAggregateRoot : class, IAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    AggregateDataModel Map(TAggregateRoot aggregate);

    AggregateDataModel Map(TAggregateRoot aggregate, ISerializer serializer);

    TAggregateRoot Map(AggregateDataModel aggregate);

    TAggregateRoot Map(AggregateDataModel aggregate, ISerializer serializer);
}