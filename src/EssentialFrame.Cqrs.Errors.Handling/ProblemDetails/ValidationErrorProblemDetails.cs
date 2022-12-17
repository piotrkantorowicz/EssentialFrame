namespace EssentialFrame.Cqrs.Errors.Handling.ProblemDetails;

public sealed class ValidationErrorProblemDetails : BaseProblemDetails
{
    private ValidationErrorProblemDetails()
    {
    }

    public ValidationErrorProblemDetails(IEnumerable<KeyValuePair<string, string>> validationErrors)
        : base("Validation error occurred")
    {
        Detail = validationErrors
                 .Select(x => new ValidationErrorModel(x.Key, x.Value))
                 .ToList();
    }

    public List<ValidationErrorModel> Detail { get; }
}

public sealed class ValidationErrorModel
{
    public ValidationErrorModel(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }

    public string PropertyName { get; }

    public string ErrorMessage { get; }
}

