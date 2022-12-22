using System.Runtime.Serialization;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
internal class AmbiguousEventHandlerException : Exception
{
    public AmbiguousEventHandlerException(string name)
        : base($"You cannot define multiple handlers for the same command ({name}).")
    {
    }

    protected AmbiguousEventHandlerException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}




