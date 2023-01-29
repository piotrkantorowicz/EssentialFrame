using EssentialFrame.Cqrs.Commands.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Errors;

public class ConflictError : ICommandError
{
    public ConflictError(string message)
    {
        Message = message;
    }

    public ConflictError(string message, string resource, string resourceId)
    {
        Message = message;
        Resource = resource;
        ResourceId = resourceId;
    }

    public ConflictError(string message, string resource, string resourceId, string conflictedResource,
        string conflictedResourceId)
    {
        Message = message;
        Resource = resource;
        ResourceId = resourceId;
        ConflictedResource = conflictedResource;
        ConflictedResourceId = conflictedResourceId;
    }

    public string Message { get; }

    public string Resource { get; }

    public string ResourceId { get; }

    public string ConflictedResource { get; }

    public string ConflictedResourceId { get; }
}