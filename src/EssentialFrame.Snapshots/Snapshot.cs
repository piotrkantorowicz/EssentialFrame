namespace EssentialFrame.Snapshots;

public class Snapshot
{
    public Guid AggregateIdentifier { get; set; }

    public int AggregateVersion { get; set; }

    public string AggregateState { get; set; }
}
