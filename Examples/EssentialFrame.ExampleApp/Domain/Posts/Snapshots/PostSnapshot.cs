using EssentialFrame.Domain.EventSourcing.Core.Snapshots;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.Posts.Snapshots;

public class PostSnapshot : Snapshot<PostIdentifier>
{
    public PostSnapshot(PostIdentifier aggregateIdentifier, int aggregateVersion, object aggregateState) : base(
        aggregateIdentifier, aggregateVersion, aggregateState)
    {
    }
}