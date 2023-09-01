using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.PostComments.DomainEvents;

public class PostCommentEditedDomainEvent : DomainEvent
{
    public PostCommentEditedDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext,
        PostCommentText text, Date editedDate) : base(aggregateIdentifier, identityContext)
    {
        Text = text;
        EditedDate = editedDate;
    }

    public PostCommentText Text { get; }

    public Date EditedDate { get; }
}