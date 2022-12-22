using EssentialFrame.Domain.Core;
using EssentialFrame.Domain.Exceptions;

namespace EssentialFrame.Domain.Aggregates;

public abstract class AggregateRoot
{
    private readonly List<IEvent> _changes = new();

    public AggregateState State { get; set; }

    public Guid AggregateIdentifier { get; set; }

    public int AggregateVersion { get; set; }

    public abstract AggregateState CreateState();

    public IEvent[] GetUncommittedChanges()
    {
        lock (_changes)
        {
            return _changes.ToArray();
        }
    }

    public IEvent[] FlushUncommittedChanges()
    {
        lock (_changes)
        {
            var changes = _changes.ToArray();

            var i = 0;

            foreach (var change in changes)
            {
                if (change.AggregateIdentifier == Guid.Empty &&
                    AggregateIdentifier == Guid.Empty)
                {
                    throw new MissingAggregateIdentifierException(GetType(), change.GetType());
                }

                if (change.AggregateIdentifier == Guid.Empty)
                {
                    change.AggregateIdentifier = AggregateIdentifier;
                }

                i++;

                change.AggregateVersion = AggregateVersion + i;
                change.EventTime = DateTimeOffset.UtcNow;
            }

            AggregateVersion += changes.Length;

            _changes.Clear();

            return changes;
        }
    }

    public void Rehydrate(IEnumerable<IEvent> history)
    {
        lock (_changes)
        {
            foreach (var change in history.ToArray())
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

    protected void Apply(IEvent change)
    {
        lock (_changes)
        {
            ApplyEvent(change);

            _changes.Add(change);
        }
    }

    protected virtual void ApplyEvent(IEvent change)
    {
        State ??= CreateState();
        State.Apply(change);
    }
}



