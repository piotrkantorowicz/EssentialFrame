namespace EssentialFrame.Cqrs.Errors.Handling.ProblemDetails;

internal class BaseProblemDetails
{
    protected BaseProblemDetails()
    {
    }

    protected BaseProblemDetails(string title) => Title = title;

    public string Title { get; init; }
}
