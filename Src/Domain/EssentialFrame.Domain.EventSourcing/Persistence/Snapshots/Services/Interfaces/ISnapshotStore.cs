using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services.Interfaces;

public interface ISnapshotStore
{
    SnapshotDataModel Get(string aggregateIdentifier);

    Task<SnapshotDataModel> GetAsync(string aggregateIdentifier, CancellationToken cancellationToken = default);

    void Save(SnapshotDataModel snapshot);

    Task SaveAsync(SnapshotDataModel snapshot, CancellationToken cancellationToken = default);

    void Box(string aggregateIdentifier);

    Task BoxAsync(string aggregateIdentifier, CancellationToken cancellationToken = default);

    SnapshotDataModel Unbox(string aggregateIdentifier);

    Task<SnapshotDataModel> UnboxAsync(string aggregateIdentifier, CancellationToken cancellationToken = default);
}