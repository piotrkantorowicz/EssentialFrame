using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.PostComments.DomainEvents;

public class PostCommentRemovedDomainEvent : DomainEvent
{
    public PostCommentRemovedDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext,
        DeletedReason removedReason, UserIdentifier removedBy) : base(aggregateIdentifier, identityContext)
    {
        RemovedReason = removedReason;
        RemovedBy = removedBy;
    }

    public DeletedReason RemovedReason { get; }

    public UserIdentifier RemovedBy { get; }
}