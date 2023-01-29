using EssentialFrame.Domain.Snapshots;

namespace EssentialFrame.Domain.Events.Snapshots.Interfaces;

public interface ISnapshotStore
{
    Snapshot Get(Guid id);

    Task<Snapshot> GetAsync(Guid id, CancellationToken cancellationToken = default);

    void Save(Snapshot snapshot);

    Task SaveAsync(Snapshot snapshot, CancellationToken cancellationToken = default);

    void Box(Guid id);

    Task BoxAsync(Guid id, CancellationToken cancellationToken = default);

    Snapshot Unbox(Guid id);

    Task<Snapshot> UnboxAsync(Guid id, CancellationToken cancellationToken = default);
}