using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Rules;

namespace EssentialFrame.Domain.Entities;

public abstract class Entity
{
    protected Entity()
    {
        ImageIdentifier = Guid.NewGuid();
    }

    protected Entity(Guid imageIdentifier)
    {
        ImageIdentifier = imageIdentifier;
    }

    public Guid ImageIdentifier { get; }

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