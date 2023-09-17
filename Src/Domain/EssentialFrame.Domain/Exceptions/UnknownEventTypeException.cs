using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

[Serializable]
public class UnknownEventTypeException : EssentialFrameException
{
    public UnknownEventTypeException(string typeName) : base(
        $"Unable to convert stored event object to domain event, because type is unknown. TypeName: {typeName}")
    {
    }
}