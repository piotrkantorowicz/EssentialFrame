using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Rules;
using EssentialFrame.Domain.Shared;

namespace EssentialFrame.Domain.Aggregates;

public abstract class AggregateRoot : DeletebleObject, IAggregateRoot
{
    private readonly List<IDomainEvent> _changes = new();

    protected AggregateRoot()
    {
        AggregateIdentifier = Guid.NewGuid();
    }

    protected AggregateRoot(Guid aggregateIdentifier)
    {
        AggregateIdentifier = aggregateIdentifier;
    }

    public Guid AggregateIdentifier { get; }
    
    public IDomainEvent[] GetUncommittedChanges()
    {
        lock (_changes)
        {
            return _changes.ToArray();
        }
    }

    public void ClearDomainEvents()
    {
        lock (_changes)
        {
            _changes.Clear();
        }
    }

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        lock (_changes)
        {
            _changes.Add(domainEvent);
        }
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