using EssentialFrame.Domain.Rules.Const;

namespace EssentialFrame.Domain.Rules.Base;

public abstract class ValueObjectBusinessRuleBase : BaseBusinessRule
{
    protected ValueObjectBusinessRuleBase(Type valueObjectType)
    {
        ValueObjectType = valueObjectType ?? throw new ArgumentNullException(nameof(valueObjectType));

        Parameters.Add(BusinessRulesUtils.ValueObjectType, valueObjectType);
    }

    protected Type ValueObjectType { get; }
}