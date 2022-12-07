namespace EssentialFrame.Snapshots;

public interface ISnapshotStore
{
    Snapshot Get(Guid id);

    void Save(Snapshot snapshot);

    void Box(Guid id);

    Snapshot Unbox(Guid id);
}
