using EssentialFrame.Cqrs.Errors.Core;

namespace EssentialFrame.Cqrs.Errors.CommandErrors;

public class DbError : ICommandError
{
    public DbError(string details)
    {
        Message = "Database error occurred.";
        Details = details;
    }

    public DbError(string message, string details)
    {
        Message = message;
        Details = details;
    }

    public string Message { get; }

    public string Details { get; }
}
