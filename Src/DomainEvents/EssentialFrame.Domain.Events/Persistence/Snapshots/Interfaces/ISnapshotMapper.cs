using EssentialFrame.Domain.Snapshots;

namespace EssentialFrame.Domain.Events.Persistence.Snapshots.Interfaces;

public interface ISnapshotMapper
{
    SnapshotDataModel Map(Snapshot snapshot);

    IReadOnlyCollection<SnapshotDataModel> Map(IEnumerable<Snapshot> snapshots);

    Snapshot Map(SnapshotDataModel snapshotDataModel);

    IReadOnlyCollection<Snapshot> Map(IEnumerable<SnapshotDataModel> snapshotDataModels);
}