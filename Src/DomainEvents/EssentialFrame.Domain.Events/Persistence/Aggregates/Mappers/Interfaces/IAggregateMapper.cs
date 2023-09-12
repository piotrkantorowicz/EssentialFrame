using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;

public interface IAggregateMapper<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    AggregateDataModel Map(IAggregateRoot<TAggregateIdentifier> aggregateRoot);
}