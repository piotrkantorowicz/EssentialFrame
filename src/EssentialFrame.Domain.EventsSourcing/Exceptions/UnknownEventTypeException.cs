using System.Runtime.Serialization;

namespace EssentialFrame.Domain.EventsSourcing.Exceptions;

[Serializable]
public class UnknownEventTypeException : Exception
{
    public UnknownEventTypeException(string typeName)
        : base($"Unable to convert stored event object to domain event, because type is unknown. TypeName: {typeName}")
    {
    }

    protected UnknownEventTypeException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
