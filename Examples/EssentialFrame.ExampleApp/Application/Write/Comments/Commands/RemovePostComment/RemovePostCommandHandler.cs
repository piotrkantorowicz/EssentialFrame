using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Persistence.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.RemovePostComment;

internal sealed class RemovePostCommandHandler : ICommandHandler<RemovePostCommand>,
    IAsyncCommandHandler<RemovePostCommand>
{
    private readonly IEventSourcingAggregateRepository<Post, PostIdentifier> _postRepository;
    private readonly IAggregateRepository<PostComment, PostCommentIdentifier> _postCommentRepository;

    public RemovePostCommandHandler(IEventSourcingAggregateRepository<Post, PostIdentifier> postRepository,
        IAggregateRepository<PostComment, PostCommentIdentifier> postCommentRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));

        _postCommentRepository =
            postCommentRepository ?? throw new ArgumentNullException(nameof(postCommentRepository));
    }

    public ICommandResult Handle(RemovePostCommand command)
    {
        PostComment postComment = _postCommentRepository.Get(PostCommentIdentifier.New(command.AggregateIdentifier));

        postComment.Remove(DeletedReason.Create(command.Reason), _postRepository, command.IdentityContext);
        _postCommentRepository.Save(postComment);

        return CommandResult.Success(postComment);
    }

    public async Task<ICommandResult> HandleAsync(RemovePostCommand command,
        CancellationToken cancellationToken = default)
    {
        PostComment postComment =
            await _postCommentRepository.GetAsync(PostCommentIdentifier.New(command.AggregateIdentifier),
                cancellationToken);

        postComment.Remove(DeletedReason.Create(command.Reason), _postRepository, command.IdentityContext);
        await _postCommentRepository.SaveAsync(postComment, cancellationToken);

        return CommandResult.Success(postComment);
    }
}