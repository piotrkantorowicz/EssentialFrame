namespace EssentialFrame.Cqrs.Errors.Handling.ProblemDetails;

public sealed class UnprocessableEntityProblemDetails : BaseProblemDetails
{
    private UnprocessableEntityProblemDetails()
    {
    }

    public UnprocessableEntityProblemDetails(string details)
        : base("Cannot process request") =>
        Detail = details;

    public string Detail { get; }
}

