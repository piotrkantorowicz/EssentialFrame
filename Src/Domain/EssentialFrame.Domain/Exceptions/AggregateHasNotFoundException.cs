using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class AggregateHasNotFoundException : EssentialFrameException
{
    public AggregateHasNotFoundException(Type aggregateType, string aggregateIdentifier) : base(
        $"Aggregate ({aggregateType.FullName}) with identifier: ({aggregateIdentifier}) hasn't been found")
    {
    }
}