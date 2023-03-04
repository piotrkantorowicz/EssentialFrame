using System.Reflection;
using EssentialFrame.Domain.Base;
using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.Exceptions;

namespace EssentialFrame.Domain.Aggregates;

public abstract class AggregateState : BusinessRuleDomainObject
{
    public void Apply(IDomainEvent domainEvent)
    {
        MethodInfo when = GetType().GetMethod("When", new[] { domainEvent.GetType() });

        if (when == null)
        {
            throw new MethodNotFoundException(GetType(), "When", domainEvent.GetType());
        }

        when.Invoke(this, new object[] { domainEvent });
    }
}