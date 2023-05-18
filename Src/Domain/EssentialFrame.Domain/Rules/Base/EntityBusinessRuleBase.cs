using EssentialFrame.Domain.Rules.Const;

namespace EssentialFrame.Domain.Rules.Base;

public abstract class EntityBusinessRuleBase : BaseBusinessRule
{
    protected EntityBusinessRuleBase(Guid entityIdentifier, Type entityType)
    {
        EntityIdentifier = entityIdentifier;
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));

        Parameters.Add(BusinessRulesUtils.EntityIdentifier, entityIdentifier);
        Parameters.Add(BusinessRulesUtils.EntityType, entityType.FullName);
    }

    protected Guid EntityIdentifier { get; }

    protected Type EntityType { get; }
}