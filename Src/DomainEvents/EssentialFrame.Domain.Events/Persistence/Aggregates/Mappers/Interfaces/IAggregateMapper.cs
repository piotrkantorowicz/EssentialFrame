using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;

public interface IAggregateMapper
{
    AggregateDataModel Map<TAggregateId>(AggregateRoot<TAggregateId> aggregateRoot)
        where TAggregateId : TypedGuidIdentifier;
}