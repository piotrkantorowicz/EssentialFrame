using EssentialFrame.Domain.Events.Persistence.Snapshots.Interfaces;
using EssentialFrame.Domain.Snapshots;

namespace EssentialFrame.Domain.Events.Persistence.Snapshots;

internal sealed class SnapshotMapper : ISnapshotMapper
{
    public SnapshotDataModel Map(Snapshot snapshot)
    {
        return new SnapshotDataModel
        {
            AggregateIdentifier = snapshot.AggregateIdentifier,
            AggregateVersion = snapshot.AggregateVersion,
            AggregateState = snapshot.AggregateState
        };
    }

    public IReadOnlyCollection<SnapshotDataModel> Map(IEnumerable<Snapshot> snapshots)
    {
        return snapshots.Select(Map).ToList();
    }

    public Snapshot Map(SnapshotDataModel snapshotDataModel)
    {
        return new Snapshot(snapshotDataModel.AggregateIdentifier, snapshotDataModel.AggregateVersion,
            snapshotDataModel.AggregateState);
    }

    public IReadOnlyCollection<Snapshot> Map(IEnumerable<SnapshotDataModel> snapshotDataModels)
    {
        return snapshotDataModels.Select(Map).ToList();
    }
}