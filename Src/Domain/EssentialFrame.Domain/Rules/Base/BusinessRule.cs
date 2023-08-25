namespace EssentialFrame.Domain.Rules.Base;

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

    protected BusinessRule(Guid domainObjectIdentifier, Type domainObjectType, string businessRuleType) : this(
        domainObjectType, businessRuleType)
    {
        DomainObjectIdentifier = domainObjectIdentifier;
        DomainObjectType = domainObjectType ?? throw new ArgumentNullException(nameof(domainObjectType));

        Parameters.Add(nameof(DomainObjectIdentifier), domainObjectIdentifier);
    }

    protected Guid DomainObjectIdentifier { get; }

    protected Type DomainObjectType { get; }

    protected string BusinessRuleType { get; }

    public abstract string Message { get; }

    public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

    public abstract bool IsBroken();
    public abstract void AddExtraParameters();
}