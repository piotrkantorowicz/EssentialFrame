using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
internal class MissingAggregateIdentifierException : EssentialFrameException
{
    public MissingAggregateIdentifierException(Type aggregateType, Type eventType) : base(
        $"The aggregate identifier is missing from both the aggregate instance ({aggregateType.FullName}) and the event instance ({eventType.FullName}).")
    {
    }
}