using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.EventSourcing.Core.Snapshots;

public class Snapshot<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    public Snapshot(TAggregateIdentifier aggregateIdentifier, int aggregateVersion, object aggregateState)
    {
        AggregateIdentifier = aggregateIdentifier;
        AggregateVersion = aggregateVersion;
        AggregateState = aggregateState;
    }

    public TAggregateIdentifier AggregateIdentifier { get; }

    public int AggregateVersion { get; }

    public object AggregateState { get; }
}