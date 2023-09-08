using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Aggregates;

public interface IAggregateRoot<out TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    TAggregateIdentifier AggregateIdentifier { get; }

    Guid? TenantIdentifier { get; }
}