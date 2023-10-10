using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;

public interface IEventSourcingAggregateMapper<TEventSourcingAggregateRoot, TAggregateIdentifier, TType>
    where TEventSourcingAggregateRoot : class, IEventSourcingAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    EventSourcingAggregateDataModel Map(TEventSourcingAggregateRoot eventSourcingAggregateRoot);
}