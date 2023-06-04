using EssentialFrame.Domain.Snapshots;

namespace EssentialFrame.Domain.Events.Persistence.Snapshots.Interfaces;

public interface ISnapshotOfflineStorage
{
    void Save(Snapshot snapshot);

    Task SaveAsync(Snapshot snapshot, CancellationToken cancellationToken = default);

    Snapshot Restore(Guid aggregateIdentifier);

    Task<Snapshot> RestoreAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);
}