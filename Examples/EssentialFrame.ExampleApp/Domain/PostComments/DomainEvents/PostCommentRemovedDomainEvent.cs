using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.PostComments.DomainEvents;

public class PostCommentRemovedDomainEvent : DomainEvent<PostCommentIdentifier>
{
    public PostCommentRemovedDomainEvent(PostCommentIdentifier aggregateIdentifier, DomainIdentity identityContext,
        DeletedReason removedReason, AuthorIdentifier removedBy) : base(aggregateIdentifier, identityContext)
    {
        RemovedReason = removedReason;
        RemovedBy = removedBy;
    }

    public DeletedReason RemovedReason { get; }

    public AuthorIdentifier RemovedBy { get; }
}