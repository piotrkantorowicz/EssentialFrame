namespace EssentialFrame.Domain.Aggregates;

public interface IAggregateRoot
{
    public Guid AggregateIdentifier { get; }
}