using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Events.Exceptions;

[Serializable]
internal class AggregateBoxingFailedException : EssentialFrameException
{
    public AggregateBoxingFailedException(Guid aggregateIdentifier, Type aggregateType, Exception innerException) :
        base(
            $"Unable to box aggregate ({aggregateType.FullName}) with id: ({aggregateIdentifier}). See inner exception for more details",
            innerException)
    {
    }
}