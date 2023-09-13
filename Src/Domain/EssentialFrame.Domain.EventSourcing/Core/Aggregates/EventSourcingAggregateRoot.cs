using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.Domain.ValueObjects.Core;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.Time;

namespace EssentialFrame.Domain.EventSourcing.Core.Aggregates;

public abstract class
    EventSourcingAggregateRoot<TAggregateIdentifier> : IEventSourcingAggregateRoot<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    private readonly List<IDomainEvent<TAggregateIdentifier>> _changes = new();

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

    public int AggregateVersion { get; private set; }

    public TenantIdentifier TenantIdentifier { get; protected set; }

    public EventSourcingAggregateState<TAggregateIdentifier> State { get; protected set; }

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

    public abstract EventSourcingAggregateState<TAggregateIdentifier> CreateState();
    public abstract void RestoreState(object aggregateState, ISerializer serializer = null);

    public IDomainEvent<TAggregateIdentifier>[] GetUncommittedChanges()
    {
        lock (_changes)
        {
            return _changes.ToArray();
        }
    }

    public IDomainEvent<TAggregateIdentifier>[] FlushUncommittedChanges()
    {
        lock (_changes)
        {
            IDomainEvent<TAggregateIdentifier>[] changes = _changes.ToArray();
            int i = 0;

            foreach (IDomainEvent<TAggregateIdentifier> change in changes)
            {
                if (change.AggregateIdentifier.Empty() || AggregateIdentifier.Empty())
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

    public void Rehydrate(IEnumerable<IDomainEvent<TAggregateIdentifier>> history)
    {
        lock (_changes)
        {
            foreach (IDomainEvent<TAggregateIdentifier> change in history)
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

    protected void Apply(IDomainEvent<TAggregateIdentifier> change)
    {
        lock (_changes)
        {
            ApplyEvent(change);

            _changes.Add(change);
        }
    }

    protected virtual void ApplyEvent(IDomainEvent<TAggregateIdentifier> change)
    {
        State ??= CreateState();
        State.Apply(change);
    }
}