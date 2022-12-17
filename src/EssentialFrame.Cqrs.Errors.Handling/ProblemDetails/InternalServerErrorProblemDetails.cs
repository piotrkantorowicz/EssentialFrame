namespace EssentialFrame.Cqrs.Errors.Handling.ProblemDetails;

public sealed class InternalServerErrorProblemDetails : BaseProblemDetails
{
    private InternalServerErrorProblemDetails()
    {
    }

    public InternalServerErrorProblemDetails(string errorDetails)
        : base("Unhandled exception occurred") =>
        Detail = errorDetails;

    public string Detail { get; }
}

