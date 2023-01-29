using EssentialFrame.Cqrs.Commands.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Errors;

public class UnexpectedError : ICommandError
{
    public UnexpectedError(string message, Exception exception)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }

    public string Message { get; }

    public Exception Exception { get; }

    public string ExceptionDetails => Exception.ToString();
}