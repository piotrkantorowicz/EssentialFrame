using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.PostComments.DomainEvents;

public class ReplyToPostCommentAddedDomainEvent : DomainEvent<PostCommentIdentifier>
{
    public ReplyToPostCommentAddedDomainEvent(PostCommentIdentifier aggregateIdentifier, DomainIdentity identityContext,
        PostIdentifier postIdentifier, AuthorIdentifier authorIdentifier,
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

    public AuthorIdentifier AuthorIdentifier { get; }

    public PostCommentIdentifier ReplyToPostCommentIdentifier { get; }

    public PostCommentText Text { get; }

    public Date CreatedDate { get; }
}