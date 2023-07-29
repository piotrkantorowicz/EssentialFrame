using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.Domain.Entities;

public abstract class Entity
{
    protected Entity(Guid entityIdentifier)
    {
        EntityIdentifier = entityIdentifier;
    }

    public Guid EntityIdentifier { get; }

    protected virtual void CheckRule(EntityBusinessRuleBase rule, bool useExtraParameters = true)
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