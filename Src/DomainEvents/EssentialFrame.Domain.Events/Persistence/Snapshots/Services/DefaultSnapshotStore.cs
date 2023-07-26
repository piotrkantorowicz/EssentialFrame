using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Models;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Services.Interfaces;

namespace EssentialFrame.Domain.Events.Persistence.Snapshots.Services;

internal sealed class DefaultSnapshotStore : ISnapshotStore
{
    private readonly ICache<Guid, SnapshotDataModel> _snapshotCache;
    private readonly ISnapshotOfflineStorage _snapshotOfflineStorage;

    public DefaultSnapshotStore(ICache<Guid, SnapshotDataModel> snapshotCache,
        ISnapshotOfflineStorage snapshotOfflineStorage)
    {
        _snapshotCache = snapshotCache ?? throw new ArgumentNullException(nameof(snapshotCache));
        _snapshotOfflineStorage =
            snapshotOfflineStorage ?? throw new ArgumentNullException(nameof(snapshotOfflineStorage));
    }

    public SnapshotDataModel Get(Guid aggregateIdentifier)
    {
        return _snapshotCache.Get(aggregateIdentifier);
    }

    public Task<SnapshotDataModel> GetAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
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

    public void Box(Guid aggregateIdentifier)
    {
        SnapshotDataModel snapshot = _snapshotCache.Get(aggregateIdentifier);

        if (snapshot is null)
        {
            throw SnapshotBoxingFailedException.SnapshotNotFound(aggregateIdentifier);
        }

        _snapshotOfflineStorage.Save(snapshot);
    }

    public async Task BoxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        SnapshotDataModel snapshot = _snapshotCache.Get(aggregateIdentifier);

        if (snapshot is null)
        {
            throw SnapshotBoxingFailedException.SnapshotNotFound(aggregateIdentifier);
        }

        await _snapshotOfflineStorage.SaveAsync(snapshot, cancellationToken);
    }

    public SnapshotDataModel Unbox(Guid aggregateIdentifier)
    {
        return _snapshotOfflineStorage.Restore(aggregateIdentifier);
    }

    public async Task<SnapshotDataModel> UnboxAsync(Guid aggregateIdentifier,
        CancellationToken cancellationToken = default)
    {
        return await _snapshotOfflineStorage.RestoreAsync(aggregateIdentifier, cancellationToken);
    }
}