using System;
using EssentialFrame.Domain.Snapshots;

namespace EssentialFrame.ExampleApp.Domain.Posts.Snapshots;

public class PostSnapshot : Snapshot
{
    public PostSnapshot(Guid aggregateIdentifier, int aggregateVersion, object aggregateState) : base(
        aggregateIdentifier, aggregateVersion, aggregateState)
    {
    }
}