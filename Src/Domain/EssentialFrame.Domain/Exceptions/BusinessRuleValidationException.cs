using EssentialFrame.Domain.Rules;
using EssentialFrame.Extensions;

namespace EssentialFrame.Domain.Exceptions;

public class BusinessRuleValidationException : Exception
{
    public BusinessRuleValidationException(IBusinessRule brokenRule) : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
        Details = brokenRule.Message;
        Parameters = brokenRule.Parameters;
    }

    public string Details { get; }

    public IBusinessRule BrokenRule { get; }

    public IDictionary<string, object> Parameters { get; }

    public override string ToString()
    {
        IEnumerable<string> parameters = Parameters.Select(p => $"Parameter: {p.Key}, Value: {p.Value}\n");

        return $"{BrokenRule?.GetTypeFullName()}: {BrokenRule?.Message} \nParameters: {parameters}";
    }
}