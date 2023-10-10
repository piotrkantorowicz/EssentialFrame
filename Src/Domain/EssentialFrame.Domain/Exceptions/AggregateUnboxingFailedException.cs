using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class AggregateUnboxingFailedException : EssentialFrameException
{
    public AggregateUnboxingFailedException(string aggregateIdentifier) : base(
        $"Unable to unbox aggregate with id: ({aggregateIdentifier}))")
    {
    }

    public AggregateUnboxingFailedException(string aggregateIdentifier, Exception innerException) : base(
        $"Unable to unbox aggregate with id: ({aggregateIdentifier}). See inner exception for more details",
        innerException)
    {
    }
}