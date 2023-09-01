using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.PostComments.DomainEvents;

public class PostCommentRemovedDomainEvent : DomainEvent
{
    public PostCommentRemovedDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext,
        DeletedReason removedReason) : base(aggregateIdentifier, identityContext)
    {
        RemovedReason = removedReason;
    }

    public DeletedReason RemovedReason { get; }
}