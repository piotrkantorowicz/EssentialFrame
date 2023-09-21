using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.Rules;
using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Time;

namespace EssentialFrame.Domain.Core.Aggregates;

public abstract class AggregateRoot<TAggregateIdentifier> : IAggregateRoot<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    private readonly List<IDomainEvent<TAggregateIdentifier>> _changes = new();

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

    public IDomainEvent<TAggregateIdentifier>[] GetUncommittedChanges()
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

    protected void AddDomainEvent(IDomainEvent<TAggregateIdentifier> domainEvent)
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