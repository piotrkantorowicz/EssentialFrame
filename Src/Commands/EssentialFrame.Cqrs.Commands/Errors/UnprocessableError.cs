using EssentialFrame.Cqrs.Commands.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Errors;

public class UnprocessableError : ICommandError
{
    public UnprocessableError(string message)
    {
        Message = message;
    }

    public string Message { get; }
}