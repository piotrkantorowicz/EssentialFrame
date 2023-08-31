using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Rules;
using EssentialFrame.Domain.Shared;

namespace EssentialFrame.Domain.Entities;

public abstract class Entity : DeletebleObject, IEntity
{
    protected Entity()
    {
        EntityIdentifier = Guid.NewGuid();
    }

    protected Entity(Guid entityIdentifier)
    {
        EntityIdentifier = entityIdentifier;
    }

    public Guid EntityIdentifier { get; }

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