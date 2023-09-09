using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers;

public class AggregateMapper : IAggregateMapper
{
    public AggregateDataModel Map<TAggregateId>(AggregateRoot<TAggregateId> aggregateRoot)
        where TAggregateId : TypedGuidIdentifier
    {
        return new AggregateDataModel
        {
            AggregateIdentifier = aggregateRoot.AggregateIdentifier.Identifier,
            AggregateVersion = aggregateRoot.AggregateVersion,
            TenantIdentifier = aggregateRoot.TenantIdentifier,
            DeletedDate = aggregateRoot.DeletedDate,
            IsDeleted = aggregateRoot.IsDeleted
        };
    }
}