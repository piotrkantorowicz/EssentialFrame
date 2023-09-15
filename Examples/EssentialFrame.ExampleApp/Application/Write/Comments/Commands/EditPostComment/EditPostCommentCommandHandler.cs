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

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.EditPostComment;

internal sealed class EditPostCommentCommandHandler : ICommandHandler<EditPostCommentCommand>,
    IAsyncCommandHandler<EditPostCommentCommand>
{
    private readonly IEventSourcingAggregateRepository<Post, PostIdentifier> _postRepository;
    private readonly IAggregateRepository<PostComment, PostCommentIdentifier> _postCommentRepository;

    public EditPostCommentCommandHandler(IEventSourcingAggregateRepository<Post, PostIdentifier> postRepository,
        IAggregateRepository<PostComment, PostCommentIdentifier> postCommentRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));

        _postCommentRepository =
            postCommentRepository ?? throw new ArgumentNullException(nameof(postCommentRepository));
    }

    public ICommandResult Handle(EditPostCommentCommand command)
    {
        PostComment postComment = _postCommentRepository.Get(PostCommentIdentifier.New(command.AggregateIdentifier));

        postComment.Edit(PostCommentText.Create(command.Comment), _postRepository, command.IdentityContext);
        _postCommentRepository.Save(postComment);

        return CommandResult.Success(postComment);
    }

    public async Task<ICommandResult> HandleAsync(EditPostCommentCommand command,
        CancellationToken cancellationToken = default)
    {
        PostComment postComment =
            await _postCommentRepository.GetAsync(PostCommentIdentifier.New(command.AggregateIdentifier),
                cancellationToken);

        postComment.Edit(PostCommentText.Create(command.Comment), _postRepository, command.IdentityContext);
        await _postCommentRepository.SaveAsync(postComment, cancellationToken);

        return CommandResult.Success(postComment);
    }
}