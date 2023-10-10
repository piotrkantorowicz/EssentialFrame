using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers;

internal sealed class
    EventSourcingAggregateMapper<TEventSourcingAggregateRoot, TAggregateIdentifier, TType> :
        IEventSourcingAggregateMapper<TEventSourcingAggregateRoot, TAggregateIdentifier, TType>
    where TEventSourcingAggregateRoot : class, IEventSourcingAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    public EventSourcingAggregateDataModel Map(TEventSourcingAggregateRoot eventSourcingAggregateRoot)
    {
        return new EventSourcingAggregateDataModel
        {
            AggregateIdentifier = eventSourcingAggregateRoot.AggregateIdentifier,
            AggregateVersion = eventSourcingAggregateRoot.AggregateVersion,
            TenantIdentifier = eventSourcingAggregateRoot.TenantIdentifier.Value
        };
    }
}