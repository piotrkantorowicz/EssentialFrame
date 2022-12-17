namespace EssentialFrame.Cqrs.Errors.Handling.ProblemDetails;

public class BaseProblemDetails
{
    protected BaseProblemDetails()
    {
    }

    protected BaseProblemDetails(string title) => Title = title;

    public string Title { get; init; }
}

