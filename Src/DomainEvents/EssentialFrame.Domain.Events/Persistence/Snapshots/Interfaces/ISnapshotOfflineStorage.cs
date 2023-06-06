namespace EssentialFrame.Domain.Events.Persistence.Snapshots.Interfaces;

public interface ISnapshotOfflineStorage
{
    void Save(SnapshotDataModel snapshot);

    Task SaveAsync(SnapshotDataModel snapshot, CancellationToken cancellationToken = default);

    SnapshotDataModel Restore(Guid aggregateIdentifier);

    Task<SnapshotDataModel> RestoreAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);
}