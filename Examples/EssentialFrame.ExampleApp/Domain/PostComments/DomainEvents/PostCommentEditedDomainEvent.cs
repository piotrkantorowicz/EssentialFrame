using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.PostComments.DomainEvents;

public class PostCommentEditedDomainEvent : DomainEvent<PostCommentIdentifier>
{
    public PostCommentEditedDomainEvent(PostCommentIdentifier aggregateIdentifier, IIdentityContext identityContext,
        PostCommentText text, Date editedDate, AuthorIdentifier editedBy) : base(aggregateIdentifier, identityContext)
    {
        Text = text;
        EditedDate = editedDate;
        EditedBy = editedBy;
    }

    public PostCommentText Text { get; }

    public Date EditedDate { get; }

    public AuthorIdentifier EditedBy { get; }
}