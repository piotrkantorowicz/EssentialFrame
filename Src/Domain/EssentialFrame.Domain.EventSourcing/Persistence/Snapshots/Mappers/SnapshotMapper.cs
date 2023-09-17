﻿using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Snapshots;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Mappers;

public class SnapshotMapper<TAggregateIdentifier> : ISnapshotMapper<TAggregateIdentifier>
    where TAggregateIdentifier : TypedGuidIdentifier
{
    public SnapshotDataModel Map(Snapshot<TAggregateIdentifier> snapshot)
    {
        return new SnapshotDataModel
        {
            AggregateIdentifier = snapshot.AggregateIdentifier.Value,
            AggregateVersion = snapshot.AggregateVersion,
            AggregateState = snapshot.AggregateState
        };
    }

    public IReadOnlyCollection<SnapshotDataModel> Map(IEnumerable<Snapshot<TAggregateIdentifier>> snapshots)
    {
        return snapshots.Select(Map).ToList();
    }

    public Snapshot<TAggregateIdentifier> Map(SnapshotDataModel snapshotDataModel)
    {
        return new Snapshot<TAggregateIdentifier>(
            TypedGuidIdentifier.New<TAggregateIdentifier>(snapshotDataModel.AggregateIdentifier),
            snapshotDataModel.AggregateVersion, snapshotDataModel.AggregateState);
    }

    public IReadOnlyCollection<Snapshot<TAggregateIdentifier>> Map(IEnumerable<SnapshotDataModel> snapshotDataModels)
    {
        return snapshotDataModels.Select(Map).ToList();
    }
}