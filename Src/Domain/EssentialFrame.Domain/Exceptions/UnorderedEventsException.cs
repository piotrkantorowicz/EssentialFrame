using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
internal class UnorderedEventsException : EssentialFrameException
{
    public UnorderedEventsException(Guid aggregate) : base(
        $"The events for this aggregate are not in the expected order ({aggregate}).")
    {
    }
}