namespace EssentialFrame.Cqrs.Errors.Handling.ProblemDetails;

public sealed class ConflictErrorProblemDetails : BaseProblemDetails
{
    private ConflictErrorProblemDetails()
    {
    }

    public ConflictErrorProblemDetails(string details)
        : base("Conflict occurred") =>
        Detail = details;

    public ConflictErrorProblemDetails(string details,
                                       string resource,
                                       string resourceId)
        : this(details)
    {
        Resource = resource;
        ResourceId = resourceId;
    }

    public ConflictErrorProblemDetails(string details,
                                       string resource,
                                       string resourceId,
                                       string conflictedResource,
                                       string conflictedResourceId)
        : this(details,
               resource,
               resourceId)
    {
        ConflictedResource = conflictedResource;
        ConflictedResourceId = conflictedResourceId;
    }

    public string Detail { get; }

    public string Resource { get; }

    public string ResourceId { get; }

    public string ConflictedResource { get; }

    public string ConflictedResourceId { get; }
}

