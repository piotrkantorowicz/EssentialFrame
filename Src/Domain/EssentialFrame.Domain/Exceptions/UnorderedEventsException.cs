using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class UnorderedEventsException : EssentialFrameException
{
    public UnorderedEventsException(string aggregateIdentifier) : base(
        $"The events for this aggregate are not in the expected order ({aggregateIdentifier})")
    {
    }
}