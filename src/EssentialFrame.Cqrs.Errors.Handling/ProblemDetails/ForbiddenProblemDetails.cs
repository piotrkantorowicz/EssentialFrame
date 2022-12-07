namespace EssentialFrame.Cqrs.Errors.Handling.ProblemDetails;

internal sealed class ForbiddenProblemDetails : BaseProblemDetails
{
    private ForbiddenProblemDetails()
    {
    }

    public ForbiddenProblemDetails(string details)
        : base("Access forbidden") =>
        Detail = details;

    public string Detail { get; }
}
