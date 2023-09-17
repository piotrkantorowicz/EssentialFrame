using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Persistence.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.AddPostComment;

internal sealed class AddPostCommentCommandHandler : ICommandHandler<AddPostCommentCommand>,
    IAsyncCommandHandler<AddPostCommentCommand>
{
    private readonly IEventSourcingAggregateRepository<Post, PostIdentifier> _postRepository;
    private readonly IAggregateRepository<PostComment, PostCommentIdentifier> _postCommentRepository;

    public AddPostCommentCommandHandler(IEventSourcingAggregateRepository<Post, PostIdentifier> postRepository,
        IAggregateRepository<PostComment, PostCommentIdentifier> postCommentRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));

        _postCommentRepository =
            postCommentRepository ?? throw new ArgumentNullException(nameof(postCommentRepository));
    }

    public ICommandResult Handle(AddPostCommentCommand command)
    {
        PostComment postComment = PostComment.Create(PostCommentIdentifier.New(command.AggregateIdentifier),
            PostIdentifier.New(command.PostIdentifier), AuthorIdentifier.New(command.UserIdentifier),
            PostCommentIdentifier.New(command.ReplyToCommentIdentifier ?? Guid.Empty),
            PostCommentText.Create(command.Comment), command.IdentityContext, _postRepository);

        _postCommentRepository.Save(postComment);

        return CommandResult.Success(postComment);
    }

    public async Task<ICommandResult> HandleAsync(AddPostCommentCommand command,
        CancellationToken cancellationToken = default)
    {
        PostComment postComment = PostComment.Create(PostCommentIdentifier.New(command.AggregateIdentifier),
            PostIdentifier.New(command.PostIdentifier), AuthorIdentifier.New(command.UserIdentifier),
            PostCommentIdentifier.New(command.ReplyToCommentIdentifier ?? Guid.Empty),
            PostCommentText.Create(command.Comment), command.IdentityContext, _postRepository);

        await _postCommentRepository.SaveAsync(postComment, cancellationToken);

        return CommandResult.Success(postComment);
    }
}