namespace EssentialFrame.Domain.Core.Rules.Base;

public abstract class BusinessRule : IBusinessRule
{
    protected BusinessRule(string businessRuleType)
    {
        BusinessRuleType = businessRuleType ?? throw new ArgumentNullException(nameof(businessRuleType));

        Parameters.Add(nameof(BusinessRuleType), businessRuleType);
    }

    protected BusinessRule(Type domainObjectType, string businessRuleType) : this(businessRuleType)
    {
        DomainObjectType = domainObjectType ?? throw new ArgumentNullException(nameof(domainObjectType));

        Parameters.Add(nameof(DomainObjectType), domainObjectType);
    }

    protected Type DomainObjectType { get; }

    protected string BusinessRuleType { get; }

    public abstract string Message { get; }

    public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

    public abstract bool IsBroken();
    public abstract void AddExtraParameters();
}