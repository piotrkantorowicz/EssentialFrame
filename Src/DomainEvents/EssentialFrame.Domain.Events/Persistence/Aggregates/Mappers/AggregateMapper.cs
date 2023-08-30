using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using AggregateRoot = EssentialFrame.Domain.Events.Core.Aggregates.AggregateRoot;

namespace EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers;

public class AggregateMapper : IAggregateMapper
{
    public AggregateDataModel Map(AggregateRoot aggregateRoot)
    {
        return new AggregateDataModel
        {
            AggregateIdentifier = aggregateRoot.AggregateIdentifier,
            AggregateVersion = aggregateRoot.AggregateVersion,
            TenantIdentifier = aggregateRoot.IdentityContext?.Tenant?.Identifier ?? Guid.Empty,
            DeletedDate = aggregateRoot.DeletedDate,
            IsDeleted = aggregateRoot.IsDeleted
        };
    }
}