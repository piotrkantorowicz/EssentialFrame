namespace EssentialFrame.Domain.Snapshots;

public class Snapshot
{
    public Snapshot(Guid aggregateIdentifier, int aggregateVersion, object aggregateState)
    {
        AggregateIdentifier = aggregateIdentifier;
        AggregateVersion = aggregateVersion;
        AggregateState = aggregateState;
    }

    public virtual Guid AggregateIdentifier { get; }

    public virtual int AggregateVersion { get; }

    public virtual object AggregateState { get; }
}