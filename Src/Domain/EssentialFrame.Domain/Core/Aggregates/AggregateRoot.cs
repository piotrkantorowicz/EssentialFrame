using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.Rules;
using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Time;

namespace EssentialFrame.Domain.Core.Aggregates;

public abstract class AggregateRoot<TAggregateIdentifier, TType> : IAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    private readonly List<IDomainEvent<TAggregateIdentifier, TType>> _changes = new();

    protected AggregateRoot(TAggregateIdentifier aggregateIdentifier)
    {
        AggregateIdentifier = aggregateIdentifier;
    }

    protected AggregateRoot(TAggregateIdentifier aggregateIdentifier, TenantIdentifier tenantIdentifier)
    {
        AggregateIdentifier = aggregateIdentifier;
        TenantIdentifier = tenantIdentifier;
    }

    public TAggregateIdentifier AggregateIdentifier { get; }

    public TenantIdentifier TenantIdentifier { get; }

    public DateTimeOffset? DeletedDate { get; private set; }
    
    public bool IsDeleted { get; private set; }
    
    public void SafeDelete()
    {
        DeletedDate = SystemClock.UtcNow;
        IsDeleted = true;
    }

    public void UnDelete()
    {
        DeletedDate = null;
        IsDeleted = false;
    }

    public IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> GetUncommittedChanges()
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

    protected void AddDomainEvent(IDomainEvent<TAggregateIdentifier, TType> domainEvent)
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