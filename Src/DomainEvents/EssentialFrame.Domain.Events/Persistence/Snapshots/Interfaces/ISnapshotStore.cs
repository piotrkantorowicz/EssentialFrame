﻿namespace EssentialFrame.Domain.Events.Persistence.Snapshots.Interfaces;

public interface ISnapshotStore
{
    SnapshotDataModel Get(Guid aggregateIdentifier);

    Task<SnapshotDataModel> GetAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);

    void Save(SnapshotDataModel snapshot);

    Task SaveAsync(SnapshotDataModel snapshot, CancellationToken cancellationToken = default);

    void Box(Guid aggregateIdentifier);

    Task BoxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);

    SnapshotDataModel Unbox(Guid aggregateIdentifier);

    Task<SnapshotDataModel> UnboxAsync(Guid aggregateIdentifier, CancellationToken cancellationToken = default);
}