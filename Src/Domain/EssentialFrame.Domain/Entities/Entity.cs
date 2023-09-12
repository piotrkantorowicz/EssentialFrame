using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Rules;
using EssentialFrame.Time;

namespace EssentialFrame.Domain.Entities;

public abstract class Entity : IEntity
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