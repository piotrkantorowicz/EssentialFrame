using System.Text;
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

    public Task<SnapshotDataModel> GetAsync(string aggregateIdentifier, CancellationToken cancellationToken)
    {
        return Task.FromResult(Get(aggregateIdentifier));
    }

    public void Save(SnapshotDataModel snapshot)
    {
        _snapshotCache.Add(snapshot.AggregateIdentifier, snapshot);
    }

    public Task SaveAsync(SnapshotDataModel snapshot, CancellationToken cancellationToken)
    {
        Save(snapshot);

        return Task.CompletedTask;
    }
    
    public void Box(string aggregateIdentifier, Encoding encoding)
    {
        SnapshotDataModel snapshot = _snapshotCache.Get(aggregateIdentifier);

        if (snapshot is null)
        {
            throw SnapshotBoxingFailedException.SnapshotNotFound(aggregateIdentifier);
        }

        _snapshotOfflineStorage.Save(snapshot, encoding);
    }

    public async Task BoxAsync(string aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken)
    {
        SnapshotDataModel snapshot = _snapshotCache.Get(aggregateIdentifier);

        if (snapshot is null)
        {
            throw SnapshotBoxingFailedException.SnapshotNotFound(aggregateIdentifier);
        }

        await _snapshotOfflineStorage.SaveAsync(snapshot, encoding, cancellationToken);
    }

    public SnapshotDataModel Unbox(string aggregateIdentifier, Encoding encoding)
    {
        return _snapshotOfflineStorage.Get(aggregateIdentifier, encoding);
    }

    public async Task<SnapshotDataModel> UnboxAsync(string aggregateIdentifier, Encoding encoding,
        CancellationToken cancellationToken)
    {
        return await _snapshotOfflineStorage.GetAsync(aggregateIdentifier, encoding, cancellationToken);
    }
}