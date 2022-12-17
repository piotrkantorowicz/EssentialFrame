namespace EssentialFrame.Cqrs.Errors.Handling.ProblemDetails;

public sealed class NotFoundProblemDetails : BaseProblemDetails
{
    private NotFoundProblemDetails()
    {
    }

    public NotFoundProblemDetails(string details)
        : base("Resource not found") =>
        Detail = details;

    public NotFoundProblemDetails(string details,
                                  string resource,
                                  string resourceId)
        : this(details)
    {
        Resource = resource;
        ResourceId = resourceId;
    }

    public string Detail { get; }

    public string Resource { get; }

    public string ResourceId { get; }
}

