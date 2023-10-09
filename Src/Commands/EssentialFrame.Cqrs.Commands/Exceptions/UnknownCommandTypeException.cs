using EssentialFrame.Exceptions;

namespace EssentialFrame.Cqrs.Commands.Exceptions;

[Serializable]
internal sealed class UnknownCommandTypeException : EssentialFrameException
{
    public UnknownCommandTypeException(string typeName) : base(
        $"Unable to convert stored command object to command, because type is unknown. TypeName: {typeName}")
    {
    }
}