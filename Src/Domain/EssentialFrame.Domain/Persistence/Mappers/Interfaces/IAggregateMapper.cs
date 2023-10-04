using EssentialFrame.Domain.Core.Aggregates;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Persistence.Mappers.Interfaces;

public interface IAggregateMapper<TAggregateIdentifier, TType> where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    AggregateDataModel Map(IAggregateRoot<TAggregateIdentifier, TType> aggregate);

    AggregateDataModel Map(IAggregateRoot<TAggregateIdentifier, TType> aggregate, ISerializer serializer);
}