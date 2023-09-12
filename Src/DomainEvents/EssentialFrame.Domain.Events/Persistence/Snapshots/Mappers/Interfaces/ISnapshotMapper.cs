using EssentialFrame.Domain.Events.Core.Snapshots;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Models;
using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.Events.Persistence.Snapshots.Mappers.Interfaces;

public interface ISnapshotMapper<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    SnapshotDataModel Map(Snapshot<TAggregateIdentifier> snapshot);

    IReadOnlyCollection<SnapshotDataModel> Map(IEnumerable<Snapshot<TAggregateIdentifier>> snapshots);

    Snapshot<TAggregateIdentifier> Map(SnapshotDataModel snapshotDataModel);

    IReadOnlyCollection<Snapshot<TAggregateIdentifier>> Map(IEnumerable<SnapshotDataModel> snapshotDataModels);
}