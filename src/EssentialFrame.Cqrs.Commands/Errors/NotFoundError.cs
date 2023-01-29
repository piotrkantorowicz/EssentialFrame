using EssentialFrame.Cqrs.Commands.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Errors;

public class NotFoundError : ICommandError
{
    public NotFoundError(string message)
    {
        Message = message;
    }

    public NotFoundError(string message, string resource, string resourceId)
    {
        Message = message;
        Resource = resource;
        ResourceId = resourceId;
    }

    public string Message { get; }

    public string Resource { get; }

    public string ResourceId { get; }
}