using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Events.Snapshots.Interfaces;
using EssentialFrame.Domain.Snapshots;

namespace EssentialFrame.Domain.Events.Snapshots;

internal sealed class DefaultSnapshotStore : ISnapshotStore
{
    private readonly ICache<Guid, Snapshot> _snapshotCache;

    public DefaultSnapshotStore(ICache<Guid, Snapshot> snapshotCache)
    {
        _snapshotCache = snapshotCache ?? throw new ArgumentNullException(nameof(snapshotCache));
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
        throw new NotImplementedException();
    }

    public Task BoxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Snapshot Unbox(Guid aggregateIdentifier)
    {
        throw new NotImplementedException();
    }

    public Task<Snapshot> UnboxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}