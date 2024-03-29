﻿using System;
using EssentialFrame.Domain.Core.Aggregates;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates.Rules;
using EssentialFrame.ExampleApp.Domain.PostComments.DomainEvents;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;

public sealed class PostComment : AggregateRoot<PostCommentIdentifier, Guid>
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
        PostCommentIdentifier replyToPostCommentIdentifier, PostCommentText text, DomainIdentity domainIdentity)
    {
        AuthorIdentifier authorIdentifier = AuthorIdentifier.New(domainIdentity.UserIdentifier);

        PostComment postComment = new(postCommentIdentifier, postIdentifier, authorIdentifier,
            replyToPostCommentIdentifier, text, TenantIdentifier.New(domainIdentity.TenantIdentifier));

        if (replyToPostCommentIdentifier.IsEmpty())
        {
            postComment.AddDomainEvent(new PostCommentAddedDomainEvent(postComment.AggregateIdentifier, domainIdentity,
                postIdentifier, authorIdentifier, text, postComment.CreatedDate));
        }
        else
        {
            postComment.AddDomainEvent(new ReplyToPostCommentAddedDomainEvent(postComment.AggregateIdentifier,
                domainIdentity, postIdentifier, authorIdentifier, replyToPostCommentIdentifier, text,
                postComment.CreatedDate));
        }

        return postComment;
    }

    public PostComment Reply(PostCommentText reply, DomainIdentity domainIdentity)
    {
        return Create(PostCommentIdentifier.New(), PostIdentifier, AggregateIdentifier, reply, domainIdentity);
    }

    public void Edit(PostCommentText text, DomainIdentity domainIdentity)
    {
        AuthorIdentifier editorIdentifier = AuthorIdentifier.New(domainIdentity.UserIdentifier);

        CheckRule(new PostCommentCanBeEditedOnlyByAuthorRule(AggregateIdentifier, GetType(), AuthorIdentifier,
            editorIdentifier));

        Text = text;
        EditedDate = Date.Create(DateTimeOffset.UtcNow);

        AddDomainEvent(new PostCommentEditedDomainEvent(AggregateIdentifier, domainIdentity, Text, EditedDate,
            editorIdentifier));
    }

    public void Remove(DeletedReason reason, DomainIdentity domainIdentity)
    {
        AuthorIdentifier removerIdentifier = AuthorIdentifier.New(domainIdentity.UserIdentifier);

        CheckRule(new PostCommentCanBeRemovedOnlyByAuthorRule(AggregateIdentifier, GetType(), AuthorIdentifier,
            removerIdentifier));

        SafeDelete();

        DeletedReason = reason;

        AddDomainEvent(new PostCommentRemovedDomainEvent(AggregateIdentifier, domainIdentity, DeletedReason,
            removerIdentifier));
    }
}