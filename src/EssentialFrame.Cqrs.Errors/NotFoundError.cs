using EssentialFrame.Cqrs.Errors.Core;

namespace EssentialFrame.Cqrs.Errors;

public class NotFoundError : IGeneralError
{
    public NotFoundError(string message) => Message = message;

    public NotFoundError(string message,
                         string resource,
                         string resourceId)
    {
        Message = message;
        Resource = resource;
        ResourceId = resourceId;
    }

    public string Message { get; }

    public string Resource { get; }

    public string ResourceId { get; }
}
