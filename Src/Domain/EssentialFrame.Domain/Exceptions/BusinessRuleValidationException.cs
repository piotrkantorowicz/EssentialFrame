using EssentialFrame.Domain.Core.Rules;
using EssentialFrame.Exceptions;

namespace EssentialFrame.Domain.Exceptions;

public class BusinessRuleValidationException : EssentialFrameException
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
        return BrokenRule.GetDetails();
    }
}