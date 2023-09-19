using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Persistence.Services.Interfaces;
using EssentialFrame.Domain.Services;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates.Rules;
using EssentialFrame.ExampleApp.Domain.PostComments.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Services;

public class PostCommentDomainService : DomainService, IPostCommentDomainService
{
    private readonly IEventSourcingAggregateRepository<Post, PostIdentifier> _postRepository;
    private readonly IAggregateRepository<PostComment, PostCommentIdentifier> _postCommentRepository;

    public PostCommentDomainService(IEventSourcingAggregateRepository<Post, PostIdentifier> aggregateRepository,
        IAggregateRepository<PostComment, PostCommentIdentifier> postCommentRepository)
    {
        _postRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));

        _postCommentRepository =
            postCommentRepository ?? throw new ArgumentNullException(nameof(postCommentRepository));
    }

    public PostComment CreateNew(PostCommentIdentifier postCommentIdentifier, PostIdentifier postIdentifier,
        PostCommentText text, IIdentityContext identityContext)
    {
        PostComment postComment = CreateNewInternal(postCommentIdentifier, postIdentifier, text, identityContext);

        _postCommentRepository.Save(postComment);

        return postComment;
    }

    public async Task<PostComment> CreateNewAsync(PostCommentIdentifier replyToPostCommentIdentifier,
        PostIdentifier postIdentifier, PostCommentText text, IIdentityContext identityContext,
        CancellationToken cancellationToken = default)
    {
        PostComment postComment =
            CreateNewInternal(replyToPostCommentIdentifier, postIdentifier, text, identityContext);

        await _postCommentRepository.SaveAsync(postComment, cancellationToken);

        return postComment;
    }

    public PostComment InReplyTo(PostCommentIdentifier replyToPostCommentIdentifier, PostCommentText text,
        IIdentityContext identityContext)
    {
        PostComment postComment = _postCommentRepository.Get(replyToPostCommentIdentifier);

        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostExistsAndHasNotBeenExpiredRule(replyToPostCommentIdentifier,
            typeof(PostComment), postComment.PostIdentifier, _postRepository));

        PostComment replyPostComment = ReplyInternal(postComment, text, identityContext);

        _postCommentRepository.Save(postComment);

        return replyPostComment;
    }

    public async Task<PostComment> InReplyToAsync(PostCommentIdentifier replyToIdentifier, PostCommentText text,
        IIdentityContext identityContext, CancellationToken cancellationToken = default)
    {
        PostComment postComment = await _postCommentRepository.GetAsync(replyToIdentifier, cancellationToken);
        PostComment replyPostComment = ReplyInternal(postComment, text, identityContext);

        await _postCommentRepository.SaveAsync(postComment, cancellationToken);

        return replyPostComment;
    }

    public PostComment Edit(PostCommentIdentifier postCommentIdentifier, PostCommentText text,
        IIdentityContext identityContext)
    {
        PostComment postComment = _postCommentRepository.Get(postCommentIdentifier);

        EditInternal(postComment, text, identityContext);

        _postCommentRepository.Save(postComment);

        return postComment;
    }

    public async Task<PostComment> EditAsync(PostCommentIdentifier postCommentIdentifier, PostCommentText text,
        IIdentityContext identityContext, CancellationToken cancellationToken = default)
    {
        PostComment postComment = await _postCommentRepository.GetAsync(postCommentIdentifier, cancellationToken);

        EditInternal(postComment, text, identityContext);

        await _postCommentRepository.SaveAsync(postComment, cancellationToken);

        return postComment;
    }

    public PostComment Remove(PostCommentIdentifier postCommentIdentifier, DeletedReason reason,
        IIdentityContext identityContext)
    {
        PostComment postComment = _postCommentRepository.Get(postCommentIdentifier);

        RemoveInternal(postComment, reason, identityContext);

        _postCommentRepository.Save(postComment);

        return postComment;
    }

    public async Task<PostComment> RemoveAsync(PostCommentIdentifier postCommentIdentifier, DeletedReason reason,
        IIdentityContext identityContext, CancellationToken cancellationToken = default)
    {
        PostComment postComment = await _postCommentRepository.GetAsync(postCommentIdentifier, cancellationToken);

        RemoveInternal(postComment, reason, identityContext);

        await _postCommentRepository.SaveAsync(postComment, cancellationToken);

        return postComment;
    }

    private PostComment CreateNewInternal(PostCommentIdentifier postCommentIdentifier, PostIdentifier postIdentifier,
        PostCommentText text, IIdentityContext identityContext)
    {
        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostExistsAndHasNotBeenExpiredRule(postCommentIdentifier,
            typeof(PostComment), postIdentifier, _postRepository));

        PostComment postComment = PostComment.Create(postCommentIdentifier, postIdentifier,
            PostCommentIdentifier.Empty(), text, identityContext);

        return postComment;
    }

    private PostComment ReplyInternal(PostComment postComment, PostCommentText text, IIdentityContext identityContext)
    {
        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostExistsAndHasNotBeenExpiredRule(postComment.AggregateIdentifier,
            postComment.GetType(), postComment.PostIdentifier, _postRepository));

        PostComment replyPostComment = postComment.Reply(text, identityContext);

        return replyPostComment;
    }

    private void EditInternal(PostComment postComment, PostCommentText text, IIdentityContext identityContext)
    {
        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostExistsAndHasNotBeenExpiredRule(postComment.AggregateIdentifier,
            postComment.GetType(), postComment.PostIdentifier, _postRepository));

        postComment.Edit(text, identityContext);
    }

    private void RemoveInternal(PostComment postComment, DeletedReason reason, IIdentityContext identityContext)
    {
        CheckRule(new PostCommentCanBeOnlyCreatedWhenPostExistsAndHasNotBeenExpiredRule(postComment.AggregateIdentifier,
            postComment.GetType(), postComment.PostIdentifier, _postRepository));

        postComment.Remove(reason, identityContext);
    }
}