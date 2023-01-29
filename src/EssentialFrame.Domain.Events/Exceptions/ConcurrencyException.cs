using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Events.Exceptions;

[Serializable]
internal class ConcurrencyException : EssentialFrameException
{
    public ConcurrencyException(Guid aggregate) : base(
        $"A concurrency violation occurred on this aggregate ({aggregate}). At least one event failed to save.")
    {
    }
}