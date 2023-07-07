using EssentialFrame.Domain.Rules.Const;

namespace EssentialFrame.Domain.Rules.Base;

public abstract class ValueObjectBusinessRuleBase : IBusinessRule
{
    protected ValueObjectBusinessRuleBase(Type valueObjectType)
    {
        ValueObjectType = valueObjectType ?? throw new ArgumentNullException(nameof(valueObjectType));

        Parameters = new Dictionary<string, object>();
        Parameters.Add(BusinessRulesUtils.ValueObjectType, valueObjectType);
    }

    protected Type ValueObjectType { get; }

    public abstract string Message { get; }

    public IDictionary<string, object> Parameters { get; }

    public abstract bool IsBroken();
    public abstract void AddExtraParameters();
}