using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Snapshots;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Mappers.Interfaces;

public interface ISnapshotMapper<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    SnapshotDataModel Map(Snapshot<TAggregateIdentifier> snapshot);

    IReadOnlyCollection<SnapshotDataModel> Map(IEnumerable<Snapshot<TAggregateIdentifier>> snapshots);

    Snapshot<TAggregateIdentifier> Map(SnapshotDataModel snapshotDataModel);

    IReadOnlyCollection<Snapshot<TAggregateIdentifier>> Map(IEnumerable<SnapshotDataModel> snapshotDataModels);
}