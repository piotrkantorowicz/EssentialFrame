using EssentialFrame.Domain.Rules.Const;

namespace EssentialFrame.Domain.Rules;

public abstract class BaseBusinessRule : IBusinessRule
{
    private const string AggregateType = "AggregateType";

    protected readonly Guid _aggregateIdentifier;
    protected readonly Type _aggregateType;

    protected BaseBusinessRule(Guid aggregateIdentifier, Type aggregateType)
    {
        _aggregateIdentifier = aggregateIdentifier;
        _aggregateType = aggregateType ?? throw new ArgumentNullException(nameof(aggregateType));

        Parameters = new Dictionary<string, object>
        {
            { BusinessRulesUtils.AggregateIdentifier, _aggregateIdentifier },
            { BusinessRulesUtils.AggregateType, _aggregateType.FullName }
        };
    }

    public abstract string Message { get; }

    public virtual IDictionary<string, object> Parameters { get; }

    public abstract bool IsBroken();

    protected abstract void AddExtraParameters();
}