using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Snapshots;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;

namespace EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Mappers.Interfaces;

public interface ISnapshotMapper<TAggregateIdentifier, TType> where TAggregateIdentifier : TypedIdentifierBase<TType>
{
    SnapshotDataModel Map(Snapshot<TAggregateIdentifier, TType> snapshot);

    IReadOnlyCollection<SnapshotDataModel> Map(IEnumerable<Snapshot<TAggregateIdentifier, TType>> snapshots);

    Snapshot<TAggregateIdentifier, TType> Map(SnapshotDataModel snapshotDataModel);

    IReadOnlyCollection<Snapshot<TAggregateIdentifier, TType>> Map(IEnumerable<SnapshotDataModel> snapshotDataModels);
}