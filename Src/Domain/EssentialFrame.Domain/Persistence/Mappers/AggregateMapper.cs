using EssentialFrame.Domain.Core.Aggregates;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Persistence.Mappers;

internal sealed class AggregateMapper<TAggregateIdentifier> : IAggregateMapper<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    public AggregateDataModel Map(IAggregateRoot<TAggregateIdentifier> aggregate)
    {
        return new AggregateDataModel
        {
            AggregateIdentifier = aggregate.AggregateIdentifier.Value,
            TenantIdentifier = aggregate.TenantIdentifier.Value,
            State = aggregate,
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted
        };
    }

    public AggregateDataModel Map(IAggregateRoot<TAggregateIdentifier> aggregate, ISerializer serializer)
    {
        return new AggregateDataModel
        {
            AggregateIdentifier = aggregate.AggregateIdentifier.Value,
            TenantIdentifier = aggregate.TenantIdentifier.Value,
            State = serializer.Serialize(aggregate),
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted
        };
    }
}