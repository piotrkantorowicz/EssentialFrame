using EssentialFrame.Cqrs.Commands.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Errors;

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