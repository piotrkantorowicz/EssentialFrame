using EssentialFrame.Domain.Core.Aggregates;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Persistence.Mappers;

internal sealed class AggregateMapper<TAggregateIdentifier, TType> : IAggregateMapper<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    public AggregateDataModel Map(IAggregateRoot<TAggregateIdentifier, TType> aggregate)

    {
        return new AggregateDataModel
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            TenantIdentifier = aggregate.TenantIdentifier,
            State = aggregate,
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted
        };
    }

    public AggregateDataModel Map(IAggregateRoot<TAggregateIdentifier, TType> aggregate, ISerializer serializer)
    {
        return new AggregateDataModel
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            TenantIdentifier = aggregate.TenantIdentifier,
            State = serializer.Serialize(aggregate),
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted
        };
    }
}