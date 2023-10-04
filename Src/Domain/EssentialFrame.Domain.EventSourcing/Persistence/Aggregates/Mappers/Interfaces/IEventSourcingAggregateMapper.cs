using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;

public interface IEventSourcingAggregateMapper<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    EventSourcingAggregateDataModel Map(
        IEventSourcingAggregateRoot<TAggregateIdentifier, TType> eventSourcingAggregateRoot);
}