using EssentialFrame.Exceptions;

namespace EssentialFrame.Cqrs.Commands.Exceptions;

[Serializable]
internal sealed class CommandNotFoundException : EssentialFrameException
{
    public CommandNotFoundException(Guid commandIdentifier) : base(
        $"Command with id: {commandIdentifier} hasn't been found in store")
    {
    }
}