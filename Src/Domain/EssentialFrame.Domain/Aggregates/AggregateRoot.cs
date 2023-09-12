using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Rules;
using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.Domain.ValueObjects.Core;
using EssentialFrame.Time;

namespace EssentialFrame.Domain.Aggregates;

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