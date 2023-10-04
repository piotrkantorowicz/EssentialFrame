using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class AggregateBoxingFailedException : EssentialFrameException
{
    public AggregateBoxingFailedException(string aggregateIdentifier, Type aggregateType, Exception innerException) :
        base(
            $"Unable to box aggregate ({aggregateType.FullName}) with id: ({aggregateIdentifier}). See inner exception for more details",
            innerException)
    {
    }
}