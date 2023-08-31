﻿using EssentialFrame.Domain.Events.Core.Snapshots;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Models;

namespace EssentialFrame.Domain.Events.Persistence.Snapshots.Mappers;

public class SnapshotMapper : ISnapshotMapper
{
    public SnapshotDataModel Map(Snapshot snapshot)
    {
        return new SnapshotDataModel
        {
            AggregateIdentifier = snapshot.AggregateIdentifier,
            AggregateVersion = snapshot.AggregateVersion,
            AggregateState = snapshot.AggregateState
        };
    }

    public IReadOnlyCollection<SnapshotDataModel> Map(IEnumerable<Snapshot> snapshots)
    {
        return snapshots.Select(Map).ToList();
    }

    public Snapshot Map(SnapshotDataModel snapshotDataModel)
    {
        return new Snapshot(snapshotDataModel.AggregateIdentifier, snapshotDataModel.AggregateVersion,
            snapshotDataModel.AggregateState);
    }

    public IReadOnlyCollection<Snapshot> Map(IEnumerable<SnapshotDataModel> snapshotDataModels)
    {
        return snapshotDataModels.Select(Map).ToList();
    }
}