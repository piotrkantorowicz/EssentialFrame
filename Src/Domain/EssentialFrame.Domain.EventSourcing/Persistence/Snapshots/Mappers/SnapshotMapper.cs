using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Snapshots;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Mappers;

public class SnapshotMapper<TAggregateIdentifier, TType> : ISnapshotMapper<TAggregateIdentifier, TType>
    where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    public SnapshotDataModel Map(Snapshot<TAggregateIdentifier, TType> snapshot)
    {
        return new SnapshotDataModel
        {
            AggregateIdentifier = snapshot.AggregateIdentifier,
            AggregateVersion = snapshot.AggregateVersion,
            AggregateState = snapshot.AggregateState
        };
    }

    public IReadOnlyCollection<SnapshotDataModel> Map(IEnumerable<Snapshot<TAggregateIdentifier, TType>> snapshots)
    {
        return snapshots.Select(Map).ToList();
    }

    public Snapshot<TAggregateIdentifier, TType> Map(SnapshotDataModel snapshotDataModel)
    {
        return new Snapshot<TAggregateIdentifier, TType>(
            TypedIdentifierBase<TType>.New<TAggregateIdentifier>(snapshotDataModel.AggregateIdentifier),
            snapshotDataModel.AggregateVersion, snapshotDataModel.AggregateState);
    }

    public IReadOnlyCollection<Snapshot<TAggregateIdentifier, TType>> Map(
        IEnumerable<SnapshotDataModel> snapshotDataModels)
    {
        return snapshotDataModels.Select(Map).ToList();
    }
}