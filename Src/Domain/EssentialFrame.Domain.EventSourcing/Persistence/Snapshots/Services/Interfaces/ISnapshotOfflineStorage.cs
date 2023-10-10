using System.Text;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services.Interfaces;

internal interface ISnapshotOfflineStorage
{
    SnapshotDataModel Get(string aggregateIdentifier, Encoding encoding);

    Task<SnapshotDataModel> GetAsync(string aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken);
    
    void Save(SnapshotDataModel snapshot, Encoding encoding);

    Task SaveAsync(SnapshotDataModel snapshot, Encoding encoding, CancellationToken cancellationToken);
}