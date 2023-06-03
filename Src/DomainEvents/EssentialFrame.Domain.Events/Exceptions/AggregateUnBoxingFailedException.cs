using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Events.Exceptions;

[Serializable]
internal class AggregateUnBoxingFailedException : EssentialFrameException
{
    public AggregateUnBoxingFailedException(Guid aggregateIdentifier, Type aggregateType, Exception innerException) :
        base(
            $"Unable to unbox aggregate ({aggregateType.FullName}) with id: ({aggregateIdentifier}). See inner exception for more details.",
            innerException)
    {
    }
}