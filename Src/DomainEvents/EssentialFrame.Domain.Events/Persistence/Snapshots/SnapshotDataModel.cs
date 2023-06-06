namespace EssentialFrame.Domain.Events.Persistence.Snapshots;

public class SnapshotDataModel
{
    public virtual Guid AggregateIdentifier { get; set; }

    public virtual int AggregateVersion { get; set; }

    public virtual object AggregateState { get; set; }
}