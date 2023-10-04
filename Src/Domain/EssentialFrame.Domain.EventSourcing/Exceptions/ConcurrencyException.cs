using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.EventSourcing.Exceptions;

[Serializable]
internal class ConcurrencyException : EssentialFrameException
{
    public ConcurrencyException(string aggregate) : base(
        $"A concurrency violation occurred on this aggregate ({aggregate}). At least one event failed to save")
    {
    }
}