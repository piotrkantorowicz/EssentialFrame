using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers;

public class AggregateMapper<TAggregateIdentifier> : IAggregateMapper<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    public AggregateDataModel Map(IAggregateRoot<TAggregateIdentifier> aggregateRoot)
    {
        return new AggregateDataModel
        {
            AggregateIdentifier = aggregateRoot.AggregateIdentifier.Value,
            AggregateVersion = aggregateRoot.AggregateVersion,
            TenantIdentifier = aggregateRoot.TenantIdentifier.Value,
            DeletedDate = aggregateRoot.DeletedDate,
            IsDeleted = aggregateRoot.IsDeleted
        };
    }
}