using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Snapshots;

namespace EssentialFrame.Domain.Events.Snapshots.Interfaces;

public interface ISnapshotOfflineStorage
{
    void Save(AggregateRoot aggregate);

    Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken = default);

    Snapshot Restore(Guid aggregateIdentifier);

    Task<Snapshot> RestoreAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);
}