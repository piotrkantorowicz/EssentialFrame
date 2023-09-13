using System;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates.Rules;
using EssentialFrame.ExampleApp.Domain.PostComments.DomainEvents;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;

public sealed class PostComment : AggregateRoot<PostCommentIdentifier>
{
    private PostComment(PostCommentIdentifier postCommentIdentifier, PostIdentifier postIdentifier,
        AuthorIdentifier authorIdentifier, PostCommentIdentifier replyToPostCommentIdentifier, PostCommentText text,
        TenantIdentifier tenantIdentifier) : base(postCommentIdentifier, tenantIdentifier)
    {
        PostIdentifier = postIdentifier;
        AuthorIdentifier = authorIdentifier;
        ReplyToPostCommentIdentifier = replyToPostCommentIdentifier;
        Text = text;
        CreatedDate = Date.Create(DateTimeOffset.UtcNow);
    }

    public PostIdentifier PostIdentifier { get; }

    public AuthorIdentifier AuthorIdentifier { get; }

    public PostCommentIdentifier ReplyToPostCommentIdentifier { get; }

    public PostCommentText Text { get; private set; }

    public Date EditedDate { get; private set; }

    public Date CreatedDate { get; }

    public DeletedReason DeletedReason { get; private set; }

    public static PostComment Create(PostCommentIdentifier postCommentIdentifier, PostIdentifier postIdentifier,
        AuthorIdentifier userIdentifier, PostCommentIdentifier replyToPostCommentIdentifier, PostCommentText text,
        IIdentityContext identityContext, IEventSourcingAggregateRepository<Post, PostIdentifier> aggregateRepository)
    {
        PostComment postComment = new(postCommentIdentifier, postIdentifier, userIdentifier,
            replyToPostCommentIdentifier, text, TenantIdentifier.New(identityContext.Tenant.Identifier));

        postComment.CheckRule(new PostCommentCanBeOnlyCreatedWhenPostHasNotBeenExpiredRule(
            postComment.AggregateIdentifier, postComment.GetType(), postIdentifier, aggregateRepository));

        if (replyToPostCommentIdentifier.Empty())
        {
            postComment.AddDomainEvent(new PostCommentAddedDomainEvent(postComment.AggregateIdentifier,
                identityContext, postIdentifier, userIdentifier, text, postComment.CreatedDate));
        }
        else
        {
            postComment.AddDomainEvent(new ReplyToPostCommentAddedDomainEvent(postComment.AggregateIdentifier,
                identityContext, postIdentifier, userIdentifier,
                replyToPostCommentIdentifier, text, postComment.CreatedDate));
        }

        return postComment;
    }

    public PostComment Reply(PostCommentText reply, AuthorIdentifier replierIdentifier,
        IIdentityContext identityContext, IEventSourcingAggregateRepository<Post, PostIdentifier> aggregateRepository)
    {
        return Create(PostCommentIdentifier.New(), PostIdentifier, replierIdentifier, AggregateIdentifier, reply,
            identityContext, aggregateRepository);
    }

    public void Edit(PostCommentText text, IEventSourcingAggregateRepository<Post, PostIdentifier> aggregateRepository,
        IIdentityContext identityContext) 
    {
        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostHasNotBeenExpiredRule(AggregateIdentifier,
            GetType(), PostIdentifier, aggregateRepository));

        AuthorIdentifier editorIdentifier = AuthorIdentifier.New(identityContext.User.Identifier);

        CheckRule(new PostCommentCanBeEditedOnlyByAuthorRule(AggregateIdentifier, GetType(),
            AuthorIdentifier, editorIdentifier));

        Text = text;
        EditedDate = Date.Create(DateTimeOffset.UtcNow);

        AddDomainEvent(new PostCommentEditedDomainEvent(AggregateIdentifier, identityContext, Text,
            EditedDate, editorIdentifier));
    }

    public void Remove(DeletedReason reason,
        IEventSourcingAggregateRepository<Post, PostIdentifier> aggregateRepository,
        IIdentityContext identityContext)
    {
        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostHasNotBeenExpiredRule(AggregateIdentifier,
            GetType(), PostIdentifier, aggregateRepository));

        AuthorIdentifier removerIdentifier = AuthorIdentifier.New(identityContext.User.Identifier);

        CheckRule(new PostCommentCanBeRemovedOnlyByAuthorRule(AggregateIdentifier, GetType(),
            AuthorIdentifier, removerIdentifier));

        SafeDelete();

        DeletedReason = reason;

        AddDomainEvent(new PostCommentRemovedDomainEvent(AggregateIdentifier, identityContext, DeletedReason,
            removerIdentifier));
    }
}