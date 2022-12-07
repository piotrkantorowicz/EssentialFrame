using EssentialFrame.Cqrs.Errors.Core;

namespace EssentialFrame.Cqrs.Errors;

public class UnexpectedError : IGeneralError
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
