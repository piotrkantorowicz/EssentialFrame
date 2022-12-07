using EssentialFrame.Cqrs.Errors.Core;

namespace EssentialFrame.Cqrs.Errors.CommandErrors;

public class UnprocessableError : ICommandError
{
    public UnprocessableError(string message) => Message = message;

    public string Message { get; }
}
