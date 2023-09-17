using EssentialFrame.Domain.Core.Aggregates;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Persistence.Mappers.Interfaces;

public interface IAggregateMapper<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    AggregateDataModel Map(IAggregateRoot<TAggregateIdentifier> aggregate);
    AggregateDataModel Map(IAggregateRoot<TAggregateIdentifier> aggregate, ISerializer serializer);
}