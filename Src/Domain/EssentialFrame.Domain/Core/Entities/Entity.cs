using EssentialFrame.Domain.Core.Rules;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Time;

namespace EssentialFrame.Domain.Core.Entities;

public abstract class Entity<TEntityIdentifier, TType> : IEntity<TEntityIdentifier, TType>
    where TEntityIdentifier : TypedIdentifierBase<TType>
{
    protected Entity(TEntityIdentifier entityIdentifier)
    {
        EntityIdentifier = entityIdentifier;
    }

    public TEntityIdentifier EntityIdentifier { get; }

    public DateTimeOffset? DeletedDate { get; private set; }

    public bool IsDeleted { get; private set; }

    public void SafeDelete()
    {
        DeletedDate = SystemClock.UtcNow;
        IsDeleted = true;
    }

    public void UnDelete()
    {
        DeletedDate = null;
        IsDeleted = false;
    }

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