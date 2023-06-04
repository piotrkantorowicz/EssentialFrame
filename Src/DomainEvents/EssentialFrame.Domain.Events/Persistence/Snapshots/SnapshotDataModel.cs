namespace EssentialFrame.Domain.Events.Persistence.Snapshots;

public class SnapshotDataModel
{
    public Guid AggregateIdentifier { get; }

    public int AggregateVersion { get; }

    public object AggregateState { get; }
}