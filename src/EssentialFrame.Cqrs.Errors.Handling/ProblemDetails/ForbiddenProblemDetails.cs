namespace EssentialFrame.Cqrs.Errors.Handling.ProblemDetails;

public sealed class ForbiddenProblemDetails : BaseProblemDetails
{
    private ForbiddenProblemDetails()
    {
    }

    public ForbiddenProblemDetails(string details)
        : base("Access forbidden") =>
        Detail = details;

    public string Detail { get; }
}
