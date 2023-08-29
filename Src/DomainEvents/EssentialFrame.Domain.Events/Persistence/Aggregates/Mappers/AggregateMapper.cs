using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;

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