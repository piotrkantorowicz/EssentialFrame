using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers;

public class EventSourcingAggregateMapper<TAggregateIdentifier> : IEventSourcingAggregateMapper<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    public EventSourcingAggregateDataModel Map(
        IEventSourcingAggregateRoot<TAggregateIdentifier> eventSourcingAggregateRoot)
    {
        return new EventSourcingAggregateDataModel
        {
            AggregateIdentifier = eventSourcingAggregateRoot.AggregateIdentifier.Value,
            AggregateVersion = eventSourcingAggregateRoot.AggregateVersion,
            TenantIdentifier = eventSourcingAggregateRoot.TenantIdentifier.Value,
            DeletedDate = eventSourcingAggregateRoot.DeletedDate,
            IsDeleted = eventSourcingAggregateRoot.IsDeleted
        };
    }
}