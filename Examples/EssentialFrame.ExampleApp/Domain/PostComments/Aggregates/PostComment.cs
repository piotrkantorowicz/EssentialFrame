﻿using System;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
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
        UserIdentifier authorIdentifier, PostCommentIdentifier replyToPostCommentIdentifier, PostCommentText text,
        Guid? tenantIdentifier) : base(postCommentIdentifier, tenantIdentifier)
    {
        PostIdentifier = postIdentifier;
        AuthorIdentifier = authorIdentifier;
        ReplyToPostCommentIdentifier = replyToPostCommentIdentifier;
        Text = text;
        CreatedDate = Date.Create(DateTimeOffset.UtcNow);
    }

    public PostIdentifier PostIdentifier { get; }

    public UserIdentifier AuthorIdentifier { get; }

    public PostCommentIdentifier ReplyToPostCommentIdentifier { get; }

    public PostCommentText Text { get; private set; }

    public Date EditedDate { get; private set; }

    public Date CreatedDate { get; }

    public DeletedReason DeletedReason { get; private set; }

    public static PostComment Create(PostCommentIdentifier postCommentIdentifier, PostIdentifier postIdentifier,
        UserIdentifier userIdentifier, PostCommentIdentifier replyToPostCommentIdentifier, PostCommentText text,
        IIdentityContext identityContext, IAggregateRepository<Post, PostIdentifier> aggregateRepository)
    {
        PostComment postComment = new(postCommentIdentifier, postIdentifier, userIdentifier,
            replyToPostCommentIdentifier, text, identityContext.Tenant?.Identifier);

        postComment.CheckRule(new PostCommentCanBeOnlyCreatedWhenPostHasNotBeenExpiredRule(
            postComment.AggregateIdentifier.Identifier, postComment.GetType(), postIdentifier, aggregateRepository));

        if (replyToPostCommentIdentifier.Empty())
        {
            postComment.AddDomainEvent(new PostCommentAddedDomainEvent(postComment.AggregateIdentifier.Identifier,
                identityContext, postIdentifier, userIdentifier, text, postComment.CreatedDate));
        }
        else
        {
            postComment.AddDomainEvent(new ReplyToPostCommentAddedDomainEvent(
                postComment.AggregateIdentifier.Identifier, identityContext, postIdentifier, userIdentifier,
                replyToPostCommentIdentifier, text, postComment.CreatedDate));
        }

        return postComment;
    }

    public PostComment Reply(PostCommentText reply, UserIdentifier replierIdentifier, IIdentityContext identityContext,
        IAggregateRepository<Post, PostIdentifier> aggregateRepository)
    {
        return Create(PostCommentIdentifier.New(), PostIdentifier, replierIdentifier, AggregateIdentifier, reply,
            identityContext, aggregateRepository);
    }

    public void Edit(PostCommentText text, IAggregateRepository<Post, PostIdentifier> aggregateRepository,
        IIdentityContext identityContext) 
    {
        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostHasNotBeenExpiredRule(AggregateIdentifier.Identifier,
            GetType(), PostIdentifier, aggregateRepository));

        UserIdentifier editorIdentifier = UserIdentifier.New(identityContext.User.Identifier);
        
        CheckRule(new PostCommentCanBeEditedOnlyByAuthorRule(AggregateIdentifier.Identifier, GetType(),
            AuthorIdentifier, editorIdentifier));

        Text = text;
        EditedDate = Date.Create(DateTimeOffset.UtcNow);

        AddDomainEvent(new PostCommentEditedDomainEvent(AggregateIdentifier.Identifier, identityContext, Text,
            EditedDate, editorIdentifier));
    }

    public void Remove(DeletedReason reason, IAggregateRepository<Post, PostIdentifier> aggregateRepository,
        IIdentityContext identityContext)
    {
        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostHasNotBeenExpiredRule(AggregateIdentifier.Identifier,
            GetType(), PostIdentifier, aggregateRepository));

        UserIdentifier removerIdentifier = UserIdentifier.New(identityContext.User.Identifier);
        
        CheckRule(new PostCommentCanBeRemovedOnlyByAuthorRule(AggregateIdentifier.Identifier, GetType(),
            AuthorIdentifier, removerIdentifier));

        SafeDelete();

        DeletedReason = reason;

        AddDomainEvent(new PostCommentRemovedDomainEvent(AggregateIdentifier.Identifier, identityContext, DeletedReason,
            removerIdentifier));
    }
}