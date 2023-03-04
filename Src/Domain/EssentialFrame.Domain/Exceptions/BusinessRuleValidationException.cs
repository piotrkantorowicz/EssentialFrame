using EssentialFrame.Domain.Rules;
using EssentialFrame.Extensions;

namespace EssentialFrame.Domain.Exceptions;

public class BusinessRuleValidationException : Exception
{
    public BusinessRuleValidationException(string ruleName, string message) : base(message)
    {
        (RuleName, Details) = (ruleName, message);
    }

    public BusinessRuleValidationException(IBusinessRule brokenRule) : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
        Details = brokenRule.Message;
    }

    public string RuleName { get; }

    public string Details { get; }

    public IBusinessRule BrokenRule { get; }

    public override string ToString()
    {
        return $"{RuleName ?? BrokenRule?.GetTypeFullName()} = {BrokenRule?.Message}";
    }
}