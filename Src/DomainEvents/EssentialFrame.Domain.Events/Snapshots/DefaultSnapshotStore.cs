using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Snapshots.Interfaces;
using EssentialFrame.Domain.Snapshots;

namespace EssentialFrame.Domain.Events.Snapshots;

internal sealed class DefaultSnapshotStore : ISnapshotStore
{
    private readonly ICache<Guid, Snapshot> _snapshotCache;
    private readonly ISnapshotOfflineStorage _snapshotOfflineStorage;

    public DefaultSnapshotStore(ICache<Guid, Snapshot> snapshotCache, ISnapshotOfflineStorage snapshotOfflineStorage)
    {
        _snapshotCache = snapshotCache ?? throw new ArgumentNullException(nameof(snapshotCache));
        _snapshotOfflineStorage =
            snapshotOfflineStorage ?? throw new ArgumentNullException(nameof(snapshotOfflineStorage));
    }

    public Snapshot Get(Guid aggregateIdentifier)
    {
        return _snapshotCache.Get(aggregateIdentifier);
    }

    public Task<Snapshot> GetAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Get(aggregateIdentifier));
    }

    public void Save(Snapshot snapshot)
    {
        _snapshotCache.Add(snapshot.AggregateIdentifier, snapshot);
    }

    public Task SaveAsync(Snapshot snapshot, CancellationToken cancellationToken = default)
    {
        Save(snapshot);

        return Task.CompletedTask;
    }

    public void Box(Guid aggregateIdentifier)
    {
        Snapshot snapshot = _snapshotCache.Get(aggregateIdentifier);

        if (snapshot is null)
        {
            throw SnapshotBoxingFailedException.SnapshotNotFound(aggregateIdentifier);
        }

        _snapshotOfflineStorage.Save(snapshot);
    }

    public async Task BoxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        Snapshot snapshot = _snapshotCache.Get(aggregateIdentifier);

        if (snapshot is null)
        {
            throw SnapshotBoxingFailedException.SnapshotNotFound(aggregateIdentifier);
        }

        await _snapshotOfflineStorage.SaveAsync(snapshot, cancellationToken);
    }

    public Snapshot Unbox(Guid aggregateIdentifier)
    {
        return _snapshotOfflineStorage.Restore(aggregateIdentifier);
    }

    public async Task<Snapshot> UnboxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        return await _snapshotOfflineStorage.RestoreAsync(aggregateIdentifier, cancellationToken); 
    }
}