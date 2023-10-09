using System.Text;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services.Interfaces;

public interface ISnapshotStore
{
    SnapshotDataModel Get(string aggregateIdentifier);

    Task<SnapshotDataModel> GetAsync(string aggregateIdentifier, CancellationToken cancellationToken);

    void Save(SnapshotDataModel snapshot);

    Task SaveAsync(SnapshotDataModel snapshot, CancellationToken cancellationToken);

    void Box(string aggregateIdentifier);

    void Box(string aggregateIdentifier, Encoding encoding);

    Task BoxAsync(string aggregateIdentifier, CancellationToken cancellationToken);

    Task BoxAsync(string aggregateIdentifier, Encoding encoding, CancellationToken cancellationToken);

    SnapshotDataModel Unbox(string aggregateIdentifier);
    SnapshotDataModel Unbox(string aggregateIdentifier, Encoding encoding);

    Task<SnapshotDataModel> UnboxAsync(string aggregateIdentifier, CancellationToken cancellationToken);

    Task<SnapshotDataModel> UnboxAsync(string aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken);
}