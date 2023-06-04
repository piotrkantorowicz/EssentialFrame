using EssentialFrame.Domain.Rules.Const;

namespace EssentialFrame.Domain.Rules.Base;

public abstract class AggregateBusinessRuleBase : IBusinessRule
{
    protected AggregateBusinessRuleBase(Guid aggregateIdentifier, Type aggregateType)
    {
        AggregateIdentifier = aggregateIdentifier;
        AggregateType = aggregateType ?? throw new ArgumentNullException(nameof(aggregateType));

        Parameters = new Dictionary<string, object>();
        Parameters.Add(BusinessRulesUtils.AggregateIdentifier, AggregateIdentifier);
        Parameters.Add(BusinessRulesUtils.AggregateType, AggregateType.FullName);
    }

    public abstract string Message { get; }

    public IDictionary<string, object> Parameters { get; }

    protected Guid AggregateIdentifier { get; }

    protected Type AggregateType { get; }

    public abstract bool IsBroken();
    public abstract void AddExtraParameters();
}