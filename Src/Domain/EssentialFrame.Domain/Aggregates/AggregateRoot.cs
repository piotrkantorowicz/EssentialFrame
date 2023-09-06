﻿using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Rules;
using EssentialFrame.Domain.Shared;
using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.Identity;

namespace EssentialFrame.Domain.Aggregates;

public abstract class AggregateRoot<T> : DeletebleObject, IAggregateRoot<T> where T : TypedGuidIdentifier
{
    private readonly List<IDomainEvent> _changes = new();

    protected AggregateRoot(T aggregateIdentifier, IIdentityContext identityContext)
    {
        AggregateIdentifier = aggregateIdentifier;
        TenantIdentifier = identityContext.Tenant.Identifier;
        IdentityContext = identityContext ?? throw new MissingIdentityContextException(GetType());
    }

    public T AggregateIdentifier { get; }

    public Guid TenantIdentifier { get; protected init; }

    public IIdentityContext IdentityContext { get; }

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