using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.PostComments.DomainEvents;

public class PostCommentAddedDomainEvent : DomainEvent<PostCommentIdentifier>
{
    public PostCommentAddedDomainEvent(PostCommentIdentifier aggregateIdentifier, IIdentityContext identityContext,
        PostIdentifier postIdentifier, UserIdentifier authorIdentifier, PostCommentText text, Date createdDate) : base(
        aggregateIdentifier, identityContext)
    {
        PostIdentifier = postIdentifier;
        AuthorIdentifier = authorIdentifier;
        Text = text;
        CreatedDate = createdDate;
    }

    public PostIdentifier PostIdentifier { get; }

    public UserIdentifier AuthorIdentifier { get; }

    public PostCommentText Text { get; }

    public Date CreatedDate { get; }
}