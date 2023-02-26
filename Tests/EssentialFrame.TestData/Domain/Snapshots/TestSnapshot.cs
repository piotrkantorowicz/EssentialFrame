using System;
using EssentialFrame.Domain.Snapshots;

namespace EssentialFrame.TestData.Domain.Snapshots;

public class TestSnapshot : Snapshot
{
    public TestSnapshot(Guid aggregateIdentifier, int aggregateVersion, object aggregateState) : base(
        aggregateIdentifier, aggregateVersion, aggregateState)
    {
    }
}