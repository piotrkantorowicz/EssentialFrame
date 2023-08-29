using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Domain.Aggregates;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _changes = new();
    private readonly IIdentityService _identityService;

    protected AggregateRoot(Guid aggregateIdentifier, int aggregateVersion)
    {
        if (aggregateIdentifier == Guid.Empty)
        {
            throw new MissingAggregateIdentifierException(GetType());
        }
        
        AggregateIdentifier = aggregateIdentifier;
        AggregateVersion = aggregateVersion;
    }

    protected AggregateRoot(Guid aggregateIdentifier, int aggregateVersion, IIdentityService identityService)
    {
        if (aggregateIdentifier == Guid.Empty)
        {
            throw new MissingAggregateIdentifierException(GetType());
        }

        _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));

        AggregateIdentifier = aggregateIdentifier;
        AggregateVersion = aggregateVersion;
    }

    public Guid AggregateIdentifier { get; }

    public int AggregateVersion { get; private set; }

    public DateTimeOffset? DeletedDate { get; private set; }

    public bool IsDeleted { get; private set; }

    public AggregateState State { get; protected set; }

    public abstract AggregateState CreateState();
    public abstract void RestoreState(object aggregateState, ISerializer serializer = null);

    public IIdentityContext GetIdentityContext()
    {
        if (_identityService is null)
        {
            throw new MissingIdentityContextException(GetType());
        }

        return _identityService.GetCurrent();
    }

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

    public void SafeDelete()
    {
        DeletedDate = DateTimeOffset.UtcNow;
        IsDeleted = true;
    }

    public void UnDelete()
    {
        DeletedDate = null;
        IsDeleted = false;
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