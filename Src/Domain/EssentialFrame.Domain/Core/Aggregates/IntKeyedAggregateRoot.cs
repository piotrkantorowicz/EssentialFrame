using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Aggregates;

public abstract class IntKeyedAggregateRoot<TAggregateIdentifier> : AggregateRoot<TAggregateIdentifier, int>
    where TAggregateIdentifier : TypedIdentifierBase<int>
{
    protected IntKeyedAggregateRoot(TAggregateIdentifier aggregateIdentifier) : base(aggregateIdentifier)
    {
    }

    protected IntKeyedAggregateRoot(TAggregateIdentifier aggregateIdentifier, TenantIdentifier tenantIdentifier) : base(
        aggregateIdentifier, tenantIdentifier)
    {
    }
}