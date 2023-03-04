using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Rules;

namespace EssentialFrame.Domain.Base;

public abstract class BusinessRuleDomainObject
{
    protected virtual void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }
}