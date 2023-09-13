namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;

public class SnapshotDataModel
{
    public virtual Guid AggregateIdentifier { get; set; }

    public virtual int AggregateVersion { get; set; }

    public virtual object AggregateState { get; set; }
}