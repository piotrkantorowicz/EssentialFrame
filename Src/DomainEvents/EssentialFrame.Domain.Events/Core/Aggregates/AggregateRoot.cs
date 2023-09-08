using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Shared;
using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.Time;

namespace EssentialFrame.Domain.Events.Core.Aggregates;

public abstract class AggregateRoot<TAggregateIdentifier> : IDeletableDomainObject, IAggregateRoot<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    private readonly List<IDomainEvent> _changes = new();

    protected AggregateRoot(TAggregateIdentifier aggregateIdentifier)
    {
        AggregateIdentifier = aggregateIdentifier;
    }

    protected AggregateRoot(TAggregateIdentifier aggregateIdentifier, int aggregateVersion)
    {
        AggregateIdentifier = aggregateIdentifier;
        AggregateVersion = aggregateVersion;
    }

    protected AggregateRoot(TAggregateIdentifier aggregateIdentifier, int aggregateVersion, Guid tenantIdentifier)
    {
        AggregateIdentifier = aggregateIdentifier;
        AggregateVersion = aggregateVersion;
        TenantIdentifier = tenantIdentifier == default ? Guid.Empty : tenantIdentifier;
    }

    public TAggregateIdentifier AggregateIdentifier { get; }

    public int AggregateVersion { get; private set; }

    public Guid? TenantIdentifier { get; protected set; }

    public AggregateState State { get; protected set; }

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
                if (change.AggregateIdentifier == Guid.Empty || AggregateIdentifier.Empty())
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