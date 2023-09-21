using System.Reflection;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.Rules;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Exceptions;

namespace EssentialFrame.Domain.EventSourcing.Core.Aggregates;

public abstract class EventSourcingAggregateState<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    public void Apply(IDomainEvent<TAggregateIdentifier> domainEvent)
    {
        MethodInfo when = GetType().GetMethod("When", new[] { domainEvent.GetType() });

        if (when == null)
        {
            throw new MethodNotFoundException(GetType(), "When", domainEvent.GetType());
        }

        when.Invoke(this, new object[] { domainEvent });
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