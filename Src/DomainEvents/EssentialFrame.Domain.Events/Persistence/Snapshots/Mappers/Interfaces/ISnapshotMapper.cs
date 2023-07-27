using EssentialFrame.Domain.Events.Persistence.Snapshots.Models;
using EssentialFrame.Domain.Snapshots;

namespace EssentialFrame.Domain.Events.Persistence.Snapshots.Mappers.Interfaces;

public interface ISnapshotMapper
{
    SnapshotDataModel Map(Snapshot snapshot);

    IReadOnlyCollection<SnapshotDataModel> Map(IEnumerable<Snapshot> snapshots);

    Snapshot Map(SnapshotDataModel snapshotDataModel);

    IReadOnlyCollection<Snapshot> Map(IEnumerable<SnapshotDataModel> snapshotDataModels);
}