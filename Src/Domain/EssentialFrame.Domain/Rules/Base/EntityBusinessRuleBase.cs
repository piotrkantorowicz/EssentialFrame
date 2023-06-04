using EssentialFrame.Domain.Rules.Const;

namespace EssentialFrame.Domain.Rules.Base;

public abstract class EntityBusinessRuleBase : IBusinessRule
{
    protected EntityBusinessRuleBase(Guid entityIdentifier, Type entityType)
    {
        EntityIdentifier = entityIdentifier;
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));

        Parameters = new Dictionary<string, object>();
        Parameters.Add(BusinessRulesUtils.EntityIdentifier, entityIdentifier);
        Parameters.Add(BusinessRulesUtils.EntityType, entityType.FullName);
    }

    public abstract string Message { get; }

    public IDictionary<string, object> Parameters { get; }

    protected Guid EntityIdentifier { get; }

    protected Type EntityType { get; }

    public abstract bool IsBroken();
    public abstract void AddExtraParameters();
}