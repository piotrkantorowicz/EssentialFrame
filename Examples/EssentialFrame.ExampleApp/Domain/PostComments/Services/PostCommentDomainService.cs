using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.Domain.Services;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates.Rules;
using EssentialFrame.ExampleApp.Domain.PostComments.Repositories;
using EssentialFrame.ExampleApp.Domain.PostComments.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.Repositories;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Services;

public class PostCommentDomainService : DomainService, IPostCommentDomainService
{
    private readonly IPostRepository _postRepository;
    private readonly IPostCommentRepository _postCommentRepository;

    public PostCommentDomainService(IPostRepository aggregateRepository, IPostCommentRepository postCommentRepository)
    {
        _postRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));

        _postCommentRepository =
            postCommentRepository ?? throw new ArgumentNullException(nameof(postCommentRepository));
    }

    public PostComment CreateNew(PostCommentIdentifier postCommentIdentifier, PostIdentifier postIdentifier,
        PostCommentText text, DomainIdentity domainIdentity)
    {
        PostComment postComment = CreateNewInternal(postCommentIdentifier, postIdentifier, text, domainIdentity);

        _postCommentRepository.Save(postComment);

        return postComment;
    }

    public async Task<PostComment> CreateNewAsync(PostCommentIdentifier replyToPostCommentIdentifier,
        PostIdentifier postIdentifier, PostCommentText text, DomainIdentity domainIdentity,
        CancellationToken cancellationToken = default)
    {
        PostComment postComment = CreateNewInternal(replyToPostCommentIdentifier, postIdentifier, text, domainIdentity);

        await _postCommentRepository.SaveAsync(postComment, cancellationToken);

        return postComment;
    }

    public PostComment InReplyTo(PostCommentIdentifier replyToPostCommentIdentifier, PostCommentText text,
        DomainIdentity domainIdentity)
    {
        PostComment postComment = _postCommentRepository.Get(replyToPostCommentIdentifier);

        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostExistsAndHasNotBeenExpiredRule(replyToPostCommentIdentifier,
            typeof(PostComment), postComment.PostIdentifier, _postRepository));

        PostComment replyPostComment = ReplyInternal(postComment, text, domainIdentity);

        _postCommentRepository.Save(postComment);

        return replyPostComment;
    }

    public async Task<PostComment> InReplyToAsync(PostCommentIdentifier replyToIdentifier, PostCommentText text,
        DomainIdentity domainIdentity, CancellationToken cancellationToken = default)
    {
        PostComment postComment = await _postCommentRepository.GetAsync(replyToIdentifier, cancellationToken);
        PostComment replyPostComment = ReplyInternal(postComment, text, domainIdentity);

        await _postCommentRepository.SaveAsync(postComment, cancellationToken);

        return replyPostComment;
    }

    public PostComment Edit(PostCommentIdentifier postCommentIdentifier, PostCommentText text,
        DomainIdentity domainIdentity)
    {
        PostComment postComment = _postCommentRepository.Get(postCommentIdentifier);

        EditInternal(postComment, text, domainIdentity);

        _postCommentRepository.Save(postComment);

        return postComment;
    }

    public async Task<PostComment> EditAsync(PostCommentIdentifier postCommentIdentifier, PostCommentText text,
        DomainIdentity domainIdentity, CancellationToken cancellationToken = default)
    {
        PostComment postComment = await _postCommentRepository.GetAsync(postCommentIdentifier, cancellationToken);

        EditInternal(postComment, text, domainIdentity);

        await _postCommentRepository.SaveAsync(postComment, cancellationToken);

        return postComment;
    }

    public PostComment Remove(PostCommentIdentifier postCommentIdentifier, DeletedReason reason,
        DomainIdentity domainIdentity)
    {
        PostComment postComment = _postCommentRepository.Get(postCommentIdentifier);

        RemoveInternal(postComment, reason, domainIdentity);

        _postCommentRepository.Save(postComment);

        return postComment;
    }

    public async Task<PostComment> RemoveAsync(PostCommentIdentifier postCommentIdentifier, DeletedReason reason,
        DomainIdentity domainIdentity, CancellationToken cancellationToken = default)
    {
        PostComment postComment = await _postCommentRepository.GetAsync(postCommentIdentifier, cancellationToken);

        RemoveInternal(postComment, reason, domainIdentity);

        await _postCommentRepository.SaveAsync(postComment, cancellationToken);

        return postComment;
    }

    private PostComment CreateNewInternal(PostCommentIdentifier postCommentIdentifier, PostIdentifier postIdentifier,
        PostCommentText text, DomainIdentity domainIdentity)
    {
        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostExistsAndHasNotBeenExpiredRule(postCommentIdentifier,
            typeof(PostComment), postIdentifier, _postRepository));

        PostComment postComment = PostComment.Create(postCommentIdentifier, postIdentifier,
            PostCommentIdentifier.Empty(), text, domainIdentity);

        return postComment;
    }

    private PostComment ReplyInternal(PostComment postComment, PostCommentText text, DomainIdentity domainIdentity)
    {
        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostExistsAndHasNotBeenExpiredRule(postComment.AggregateIdentifier,
            postComment.GetType(), postComment.PostIdentifier, _postRepository));

        PostComment replyPostComment = postComment.Reply(text, domainIdentity);

        return replyPostComment;
    }

    private void EditInternal(PostComment postComment, PostCommentText text, DomainIdentity domainIdentity)
    {
        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostExistsAndHasNotBeenExpiredRule(postComment.AggregateIdentifier,
            postComment.GetType(), postComment.PostIdentifier, _postRepository));

        postComment.Edit(text, domainIdentity);
    }

    private void RemoveInternal(PostComment postComment, DeletedReason reason, DomainIdentity domainIdentity)
    {
        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostExistsAndHasNotBeenExpiredRule(postComment.AggregateIdentifier,
            postComment.GetType(), postComment.PostIdentifier, _postRepository));

        postComment.Remove(reason, domainIdentity);
    }
}