using System.Runtime.Serialization;

namespace EssentialFrame.Cqrs.Commands.Exceptions;

[Serializable]
public class UnknownCommandTypeException : Exception
{
    public UnknownCommandTypeException(string typeName)
        : base($"Unable to convert stored command object to command, because type is unknown. TypeName: {typeName}")
    {
    }

    protected UnknownCommandTypeException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
