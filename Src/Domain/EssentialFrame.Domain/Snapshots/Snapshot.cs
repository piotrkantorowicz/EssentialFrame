namespace EssentialFrame.Domain.Snapshots;

public class Snapshot
{
    public virtual Guid AggregateIdentifier { get; set; }

    public virtual int AggregateVersion { get; set; }

    public virtual string AggregateState { get; set; }
}