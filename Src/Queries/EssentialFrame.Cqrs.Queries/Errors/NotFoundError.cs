using EssentialFrame.Cqrs.Queries.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Queries.Errors;

public class NotFoundError : IQueryError
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