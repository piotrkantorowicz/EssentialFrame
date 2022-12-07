namespace EssentialFrame.Cqrs.Errors.Handling.ProblemDetails;

internal sealed class UnprocessableEntityProblemDetails : BaseProblemDetails
{
    private UnprocessableEntityProblemDetails()
    {
    }

    public UnprocessableEntityProblemDetails(string details)
        : base("Cannot process request") =>
        Detail = details;

    public string Detail { get; }
}
