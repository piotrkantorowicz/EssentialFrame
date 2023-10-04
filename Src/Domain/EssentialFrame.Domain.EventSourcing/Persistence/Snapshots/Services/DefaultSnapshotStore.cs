using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.EventSourcing.Exceptions;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services.Interfaces;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services;

internal sealed class DefaultSnapshotStore : ISnapshotStore
{
    private readonly ICache<string, SnapshotDataModel> _snapshotCache;
    private readonly ISnapshotOfflineStorage _snapshotOfflineStorage;

    public DefaultSnapshotStore(ICache<string, SnapshotDataModel> snapshotCache,
        ISnapshotOfflineStorage snapshotOfflineStorage)
    {
        _snapshotCache = snapshotCache ?? throw new ArgumentNullException(nameof(snapshotCache));
        _snapshotOfflineStorage =
            snapshotOfflineStorage ?? throw new ArgumentNullException(nameof(snapshotOfflineStorage));
    }

    public SnapshotDataModel Get(string aggregateIdentifier)
    {
        return _snapshotCache.Get(aggregateIdentifier);
    }

    public Task<SnapshotDataModel> GetAsync(string aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Get(aggregateIdentifier));
    }

    public void Save(SnapshotDataModel snapshot)
    {
        _snapshotCache.Add(snapshot.AggregateIdentifier, snapshot);
    }

    public Task SaveAsync(SnapshotDataModel snapshot, CancellationToken cancellationToken = default)
    {
        Save(snapshot);

        return Task.CompletedTask;
    }

    public void Box(string aggregateIdentifier)
    {
        SnapshotDataModel snapshot = _snapshotCache.Get(aggregateIdentifier);

        if (snapshot is null)
        {
            throw SnapshotBoxingFailedException.SnapshotNotFound(aggregateIdentifier);
        }

        _snapshotOfflineStorage.Save(snapshot);
    }

    public async Task BoxAsync(string aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        SnapshotDataModel snapshot = _snapshotCache.Get(aggregateIdentifier);

        if (snapshot is null)
        {
            throw SnapshotBoxingFailedException.SnapshotNotFound(aggregateIdentifier);
        }

        await _snapshotOfflineStorage.SaveAsync(snapshot, cancellationToken);
    }

    public SnapshotDataModel Unbox(string aggregateIdentifier)
    {
        return _snapshotOfflineStorage.Restore(aggregateIdentifier);
    }

    public async Task<SnapshotDataModel> UnboxAsync(string aggregateIdentifier,
        CancellationToken cancellationToken = default)
    {
        return await _snapshotOfflineStorage.RestoreAsync(aggregateIdentifier, cancellationToken);
    }
}