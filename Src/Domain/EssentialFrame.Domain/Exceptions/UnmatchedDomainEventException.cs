using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class UnmatchedDomainEventException : EssentialFrameException
{
    public UnmatchedDomainEventException(Type aggregateType, Type eventType, string aggregateIdentifier,
        string expectedAggregateIdentifier) :
        base(
            $"Aggregate ({aggregateType.FullName}) with identifier: ({aggregateIdentifier}) doesn't match provided domain event ({eventType.FullName}) with expected aggregate identifier: ({expectedAggregateIdentifier})")
    {
    }
}