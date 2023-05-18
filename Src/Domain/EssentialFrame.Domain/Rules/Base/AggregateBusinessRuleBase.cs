using EssentialFrame.Domain.Rules.Const;

namespace EssentialFrame.Domain.Rules.Base;

public abstract class AggregateBusinessRuleBase : BaseBusinessRule
{
    protected AggregateBusinessRuleBase(Guid aggregateIdentifier, Type aggregateType)
    {
        AggregateIdentifier = aggregateIdentifier;
        AggregateType = aggregateType ?? throw new ArgumentNullException(nameof(aggregateType));

        Parameters.Add(BusinessRulesUtils.AggregateIdentifier, AggregateIdentifier);
        Parameters.Add(BusinessRulesUtils.AggregateType, AggregateType.FullName);
    }

    protected Guid AggregateIdentifier { get; }

    protected Type AggregateType { get; }
}