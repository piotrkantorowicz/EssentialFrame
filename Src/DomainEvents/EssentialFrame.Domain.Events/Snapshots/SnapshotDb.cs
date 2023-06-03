namespace EssentialFrame.Domain.Events.Snapshots;

public class SnapshotDb
{
    public Guid AggregateIdentifier { get; }

    public int AggregateVersion { get; }

    public object AggregateState { get; }
}