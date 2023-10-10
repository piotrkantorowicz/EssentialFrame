using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class AggregateNotFoundException : EssentialFrameException
{
    public AggregateNotFoundException(Type aggregateType, string aggregateIdentifier) : base(
        $"Aggregate ({aggregateType.FullName}) with identifier: ({aggregateIdentifier}) hasn't been found")
    {
    }
}