using System.Text;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services.Interfaces;

internal interface ISnapshotOfflineStorage
{
    void Save(SnapshotDataModel snapshot, Encoding encoding);

    Task SaveAsync(SnapshotDataModel snapshot, Encoding encoding, CancellationToken cancellationToken = default);

    SnapshotDataModel Restore(string aggregateIdentifier, Encoding encoding);

    Task<SnapshotDataModel> RestoreAsync(string aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken = default);
}