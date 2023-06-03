namespace EssentialFrame.Domain.Snapshots;

public class Snapshot
{
    public Snapshot(Guid aggregateIdentifier, int aggregateVersion, object aggregateState)
    {
        AggregateIdentifier = aggregateIdentifier;
        AggregateVersion = aggregateVersion;
        AggregateState = aggregateState;
    }

    public Guid AggregateIdentifier { get; }

    public int AggregateVersion { get; }

    public object AggregateState { get; }
}