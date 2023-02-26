﻿using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Aggregates;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _changes = new();

    protected AggregateRoot(Guid aggregateIdentifier, int aggregateVersion)
    {
        if (aggregateIdentifier == Guid.Empty)
        {
            throw new MissingAggregateIdentifierException(GetType());
        }

        AggregateIdentifier = aggregateIdentifier;
        AggregateVersion = aggregateVersion;
    }

    public Guid AggregateIdentifier { get; }

    public int AggregateVersion { get; private set; }

    public AggregateState State { get; protected set; }

    public abstract AggregateState CreateState();
    public abstract void RestoreState(object aggregateState, ISerializer serializer = null);

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
                if (change.AggregateIdentifier == Guid.Empty || AggregateIdentifier == Guid.Empty)
                {
                    throw new MissingAggregateIdentifierException(GetType(), change.GetType());
                }

                i++;

                change.AdjustAggregateVersion(AggregateIdentifier, AggregateVersion + i);
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
            foreach (IDomainEvent change in history)
            {
                if (change.AggregateIdentifier != AggregateIdentifier)
                {
                    throw new UnmatchedDomainEventException(GetType(), change.GetType(), AggregateIdentifier,
                        change.AggregateIdentifier);
                }

                if (change.AggregateVersion != AggregateVersion + 1)
                {
                    throw new UnorderedEventsException(change.AggregateIdentifier);
                }

                ApplyEvent(change);

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