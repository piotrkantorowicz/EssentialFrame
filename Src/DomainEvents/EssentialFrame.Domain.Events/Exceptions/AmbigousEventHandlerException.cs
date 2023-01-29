using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Events.Exceptions;

[Serializable]
internal class AmbiguousEventHandlerException : EssentialFrameException
{
    public AmbiguousEventHandlerException(string name) : base(
        $"You cannot define multiple handlers for the same command ({name}).")
    {
    }
}