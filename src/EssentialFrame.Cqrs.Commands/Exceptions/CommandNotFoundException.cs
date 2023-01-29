using EssentialFrame.Exceptions;

namespace EssentialFrame.Cqrs.Commands.Exceptions;

[Serializable]
public class CommandNotFoundException : EssentialFrameException
{
    public CommandNotFoundException(Guid commandIdentifier) : base(
        $"Command with id: {commandIdentifier} hasn't been found in store.")
    {
    }
}