namespace EssentialFrame.Cqrs.Errors.Handling.ProblemDetails;

public sealed class DomainRuleValidationProblemDetails : BaseProblemDetails
{
    private DomainRuleValidationProblemDetails()
    {
    }

    public DomainRuleValidationProblemDetails(string brokenRule, string details)
        : base("Business rule broken") =>
        Detail = $"{brokenRule}. {details}";

    public DomainRuleValidationProblemDetails(string brokenRule,
                                              string details,
                                              IDictionary<string, string> additionalDetails)
        : this(brokenRule, details) =>
        AdditionalDetails = additionalDetails;

    public string Detail { get; }

    public IDictionary<string, string> AdditionalDetails { get; }
}
