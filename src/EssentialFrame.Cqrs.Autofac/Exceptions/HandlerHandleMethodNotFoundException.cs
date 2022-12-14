using System.Runtime.Serialization;

namespace EssentialFrame.Cqrs.Autofac.Exceptions;

[Serializable]
internal class HandlerHandleMethodNotFoundException : Exception
{
    public HandlerHandleMethodNotFoundException(string name)
        : base($"Handler method for command or query: ({name}) hasn't been found. It occurred, because likely handler hasn't has method implementation.")
    {
    }

    protected HandlerHandleMethodNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
