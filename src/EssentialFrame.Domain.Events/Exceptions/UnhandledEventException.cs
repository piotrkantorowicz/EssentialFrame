using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Events.Exceptions;

[Serializable]
internal class UnhandledEventException : EssentialFrameException
{
    public UnhandledEventException(string name) : base(
        $"You must register at least one handler for this event ({name}).")
    {
    }
}