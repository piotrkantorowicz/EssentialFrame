using System.Runtime.Serialization;

namespace EssentialFrame.Cqrs.Autofac.Exceptions;

[Serializable]
internal class HandlerNotFoundException : Exception
{
    public HandlerNotFoundException(string name)
        : base($"Unable to dispatch command or query: ({name}), because no handler has been registered for these types.")
    {
    }

    protected HandlerNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
