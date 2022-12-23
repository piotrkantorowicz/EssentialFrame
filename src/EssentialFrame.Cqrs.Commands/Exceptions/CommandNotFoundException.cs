using System.Runtime.Serialization;

namespace EssentialFrame.Cqrs.Commands.Exceptions;

[Serializable]
public class CommandNotFoundException : Exception
{
    public CommandNotFoundException(Guid commandIdentifier)
        : base($"Command with id: {commandIdentifier} hasn't been found in store.")
    {
    }

    protected CommandNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
