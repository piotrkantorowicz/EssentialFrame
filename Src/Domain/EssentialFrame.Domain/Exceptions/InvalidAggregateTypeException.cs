using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class InvalidAggregateTypeException : EssentialFrameException
{
    public InvalidAggregateTypeException(Type aggregateType, Type expectedType, string aggregateIdentifier) : base(
        $"Aggregate ({aggregateType.FullName}) with identifier: ({aggregateIdentifier}) is not of type ({expectedType.FullName})")
    {
    }
}