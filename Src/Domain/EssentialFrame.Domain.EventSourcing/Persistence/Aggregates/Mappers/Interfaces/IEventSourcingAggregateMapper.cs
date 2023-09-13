using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;

public interface IEventSourcingAggregateMapper<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    EventSourcingAggregateDataModel Map(IEventSourcingAggregateRoot<TAggregateIdentifier> eventSourcingAggregateRoot);
}