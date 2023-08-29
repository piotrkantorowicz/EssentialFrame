using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class MissingAggregateIdentifierException : EssentialFrameException
{
    public MissingAggregateIdentifierException(Type aggregateType) : base(
        $"The aggregate identifier is missing from the aggregate instance ({aggregateType.FullName})")
    {
    }

    public MissingAggregateIdentifierException(Type aggregateType, Type eventType) : base(
        $"The aggregate identifier is missing from aggregate instance ({aggregateType.FullName}) or the event instance ({eventType.FullName})")
    {
    }
}