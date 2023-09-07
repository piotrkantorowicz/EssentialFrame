using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Aggregates;

public interface IAggregateRoot<out T> where T : TypedGuidIdentifier
{
    T AggregateIdentifier { get; }

    Guid? TenantIdentifier { get; }
}