using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Events.Events;
using EssentialFrame.Domain.Events.Snapshots.Interfaces;
using EssentialFrame.Domain.Snapshots;

namespace EssentialFrame.Domain.Events.Snapshots;

internal sealed class DefaultSnapshotStore : ISnapshotStore
{
    private readonly ICache<Guid, DomainEventDao> _snapshotCache;

    public DefaultSnapshotStore(ICache<Guid, DomainEventDao> snapshotCache)
    {
        _snapshotCache = snapshotCache ?? throw new ArgumentNullException(nameof(snapshotCache));
    }
    
    public Snapshot Get(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Snapshot> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Save(Snapshot snapshot)
    {
        throw new NotImplementedException();
    }

    public Task SaveAsync(Snapshot snapshot, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void Box(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task BoxAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Snapshot Unbox(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Snapshot> UnboxAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}