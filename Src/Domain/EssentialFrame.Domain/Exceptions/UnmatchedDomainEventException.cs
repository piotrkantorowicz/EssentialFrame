using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class UnmatchedDomainEventException : EssentialFrameException
{
    public UnmatchedDomainEventException(Type aggregateType, Type eventType, Guid aggregateId, Guid eventAggregateId) :
        base(
            $"Aggregate ({aggregateType.FullName}) with identifier: ({aggregateId}) doesn't match provided domain event ({eventType.FullName}) with expected aggregate identifier: ({eventAggregateId}).")
    {
    }
}