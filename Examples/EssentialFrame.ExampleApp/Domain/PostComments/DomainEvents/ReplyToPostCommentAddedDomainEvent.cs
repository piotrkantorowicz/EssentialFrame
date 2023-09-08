using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.PostComments.DomainEvents;

public class ReplyToPostCommentAddedDomainEvent : DomainEvent
{
    public ReplyToPostCommentAddedDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext,
        PostIdentifier postIdentifier, UserIdentifier authorIdentifier,
        PostCommentIdentifier replyToPostCommentIdentifier, PostCommentText text, Date createdDate) : base(
        aggregateIdentifier, identityContext)
    {
        PostIdentifier = postIdentifier;
        AuthorIdentifier = authorIdentifier;
        ReplyToPostCommentIdentifier = replyToPostCommentIdentifier;
        Text = text;
        CreatedDate = createdDate;
    }

    public PostIdentifier PostIdentifier { get; }

    public UserIdentifier AuthorIdentifier { get; }

    public PostCommentIdentifier ReplyToPostCommentIdentifier { get; }

    public PostCommentText Text { get; }

    public Date CreatedDate { get; }
}