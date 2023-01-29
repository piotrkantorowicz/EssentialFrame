﻿using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.Exceptions;

namespace EssentialFrame.Domain.Aggregates;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _changes = new();

    public AggregateState State { get; set; }

    public Guid AggregateIdentifier { get; set; }

    public int AggregateVersion { get; set; }

    public abstract AggregateState CreateState();

    public IDomainEvent[] GetUncommittedChanges()
    {
        lock (_changes)
        {
            return _changes.ToArray();
        }
    }

    public IDomainEvent[] FlushUncommittedChanges()
    {
        lock (_changes)
        {
            IDomainEvent[] changes = _changes.ToArray();

            int i = 0;

            foreach (IDomainEvent change in changes)
            {
                if (change.AggregateIdentifier == Guid.Empty && AggregateIdentifier == Guid.Empty)
                {
                    throw new MissingAggregateIdentifierException(GetType(), change.GetType());
                }

                i++;

                change.AdjustToAggregate(AggregateIdentifier, AggregateVersion + i);
            }

            AggregateVersion += changes.Length;

            _changes.Clear();

            return changes;
        }
    }

    public void Rehydrate(IEnumerable<IDomainEvent> history)
    {
        lock (_changes)
        {
            foreach (IDomainEvent change in history.ToArray())
            {
                if (change.AggregateVersion != AggregateVersion + 1)
                {
                    throw new UnorderedEventsException(change.AggregateIdentifier);
                }

                ApplyEvent(change);

                AggregateIdentifier = change.AggregateIdentifier;
                AggregateVersion++;
            }
        }
    }

    protected void Apply(IDomainEvent change)
    {
        lock (_changes)
        {
            ApplyEvent(change);

            _changes.Add(change);
        }
    }

    protected virtual void ApplyEvent(IDomainEvent change)
    {
        State ??= CreateState();
        State.Apply(change);
    }
}