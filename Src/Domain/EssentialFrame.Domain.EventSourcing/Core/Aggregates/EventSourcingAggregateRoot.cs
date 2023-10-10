using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.EventSourcing.Core.Aggregates;

public abstract class
    EventSourcingAggregateRoot<TAggregateIdentifier, TType> : IEventSourcingAggregateRoot<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    private readonly List<IDomainEvent<TAggregateIdentifier, TType>> _changes = new();

    protected EventSourcingAggregateRoot(TAggregateIdentifier aggregateIdentifier)
    {
        AggregateIdentifier = aggregateIdentifier;
    }

    protected EventSourcingAggregateRoot(TAggregateIdentifier aggregateIdentifier, int aggregateVersion)
    {
        AggregateIdentifier = aggregateIdentifier;
        AggregateVersion = aggregateVersion;
    }

    protected EventSourcingAggregateRoot(TAggregateIdentifier aggregateIdentifier, int aggregateVersion,
        TenantIdentifier tenantIdentifier)
    {
        AggregateIdentifier = aggregateIdentifier;
        AggregateVersion = aggregateVersion;
        TenantIdentifier = tenantIdentifier;
    }

    public TAggregateIdentifier AggregateIdentifier { get; }

    public int AggregateVersion { get; protected set; }

    public TenantIdentifier TenantIdentifier { get; }

    public EventSourcingAggregateState<TAggregateIdentifier, TType> State { get; protected set; }

    public abstract EventSourcingAggregateState<TAggregateIdentifier, TType> CreateState();

    public virtual void RestoreState(object aggregateState, int version)
    {
        State = (EventSourcingAggregateState<TAggregateIdentifier, TType>)aggregateState;
        AggregateVersion = version;
    }

    public abstract void RestoreState(string aggregateStateString, int version, ISerializer serializer);

    public IDomainEvent<TAggregateIdentifier, TType>[] GetUncommittedChanges()
    {
        lock (_changes)
        {
            return _changes.ToArray();
        }
    }

    public IDomainEvent<TAggregateIdentifier, TType>[] FlushUncommittedChanges()
    {
        lock (_changes)
        {
            IDomainEvent<TAggregateIdentifier, TType>[] changes = _changes.ToArray();
            int i = 0;

            foreach (IDomainEvent<TAggregateIdentifier, TType> change in changes)
            {
                if (change.AggregateIdentifier.IsEmpty() || AggregateIdentifier.IsEmpty())
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

    public void Rehydrate(IEnumerable<IDomainEvent<TAggregateIdentifier, TType>> history)
    {
        lock (_changes)
        {
            foreach (IDomainEvent<TAggregateIdentifier, TType> change in history)
            {
                if (change.AggregateIdentifier != AggregateIdentifier)
                {
                    throw new UnmatchedDomainEventException(GetType(), change.GetType(), AggregateIdentifier.ToString(),
                        change.AggregateIdentifier.ToString());
                }

                if (change.AggregateVersion != AggregateVersion + 1)
                {
                    throw new UnorderedEventsException(change.AggregateIdentifier.ToString());
                }

                ApplyEvent(change);

                AggregateVersion++;
            }
        }
    }

    protected void Apply(IDomainEvent<TAggregateIdentifier, TType> change)
    {
        lock (_changes)
        {
            ApplyEvent(change);

            _changes.Add(change);
        }
    }

    protected virtual void ApplyEvent(IDomainEvent<TAggregateIdentifier, TType> change)
    {
        State ??= CreateState();
        State.Apply(change);
    }
}