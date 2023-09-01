using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.Domain.Aggregates;

public interface IAggregateRoot<out T> where T : TypedGuidIdentifier
{
    public T AggregateIdentifier { get; }
}