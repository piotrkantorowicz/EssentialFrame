using EssentialFrame.Domain.Snapshots;

namespace EssentialFrame.Domain.Events.Snapshots.Interfaces;

public interface ISnapshotStore
{
    Snapshot Get(Guid aggregateIdentifier);

    Task<Snapshot> GetAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);

    void Save(Snapshot snapshot);

    Task SaveAsync(Snapshot snapshot, CancellationToken cancellationToken = default);

    void Box(Guid aggregateIdentifier);

    Task BoxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);

    Snapshot Unbox(Guid aggregateIdentifier);

    Task<Snapshot> UnboxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);
}