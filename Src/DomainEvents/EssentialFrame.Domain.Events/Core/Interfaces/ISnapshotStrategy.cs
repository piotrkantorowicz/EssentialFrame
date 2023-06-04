using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.Events.Core.Interfaces;

public interface ISnapshotStrategy
{
    bool ShouldTakeSnapShot(AggregateRoot aggregate);
}