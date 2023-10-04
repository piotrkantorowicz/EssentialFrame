using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.Aggregates;

public abstract class StringKeyedAggregateRoot<TAggregateIdentifier> : AggregateRoot<TAggregateIdentifier, string>
    where TAggregateIdentifier : TypedIdentifierBase<string>
{
    protected StringKeyedAggregateRoot(TAggregateIdentifier aggregateIdentifier) : base(aggregateIdentifier)
    {
    }

    protected StringKeyedAggregateRoot(TAggregateIdentifier aggregateIdentifier, TenantIdentifier tenantIdentifier) :
        base(aggregateIdentifier, tenantIdentifier)
    {
    }
}