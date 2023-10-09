using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers;

internal sealed class
    EventSourcingAggregateMapper<TAggregateIdentifier, TType> : IEventSourcingAggregateMapper<TAggregateIdentifier,
        TType> where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    public EventSourcingAggregateDataModel Map(
        IEventSourcingAggregateRoot<TAggregateIdentifier, TType> eventSourcingAggregateRoot)
    {
        return new EventSourcingAggregateDataModel
        {
            AggregateIdentifier = eventSourcingAggregateRoot.AggregateIdentifier,
            AggregateVersion = eventSourcingAggregateRoot.AggregateVersion,
            TenantIdentifier = eventSourcingAggregateRoot.TenantIdentifier.Value,
            DeletedDate = eventSourcingAggregateRoot.DeletedDate,
            IsDeleted = eventSourcingAggregateRoot.IsDeleted
        };
    }
}