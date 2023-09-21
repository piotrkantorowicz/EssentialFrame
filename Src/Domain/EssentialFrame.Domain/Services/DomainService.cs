using EssentialFrame.Domain.Core.Rules;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Services.Interfaces;

namespace EssentialFrame.Domain.Services;

public abstract class DomainService : IDomainService
{
    protected virtual void CheckRule(IBusinessRule rule, bool useExtraParameters = true)
    {
        if (!rule.IsBroken())
        {
            return;
        }

        if (useExtraParameters)
        {
            rule.AddExtraParameters();
        }

        throw new BusinessRuleValidationException(rule);
    }
}