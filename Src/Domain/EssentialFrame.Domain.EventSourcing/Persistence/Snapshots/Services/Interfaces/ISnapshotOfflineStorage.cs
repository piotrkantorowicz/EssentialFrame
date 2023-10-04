using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services.Interfaces;

public interface ISnapshotOfflineStorage
{
    void Save(SnapshotDataModel snapshot);

    Task SaveAsync(SnapshotDataModel snapshot, CancellationToken cancellationToken = default);

    SnapshotDataModel Restore(string aggregateIdentifier);

    Task<SnapshotDataModel> RestoreAsync(string aggregateIdentifier, CancellationToken cancellationToken = default);
}