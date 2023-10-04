using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Aggregates;

public abstract class GuidKeyedAggregateRoot<TAggregateIdentifier> : AggregateRoot<TAggregateIdentifier, Guid>
    where TAggregateIdentifier : TypedIdentifierBase<Guid>
{
    protected GuidKeyedAggregateRoot(TAggregateIdentifier aggregateIdentifier) : base(aggregateIdentifier)
    {
    }

    protected GuidKeyedAggregateRoot(TAggregateIdentifier aggregateIdentifier, TenantIdentifier tenantIdentifier) :
        base(aggregateIdentifier, tenantIdentifier)
    {
    }
}