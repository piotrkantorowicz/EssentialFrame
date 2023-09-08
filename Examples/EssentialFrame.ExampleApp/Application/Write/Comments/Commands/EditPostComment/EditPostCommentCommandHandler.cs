using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.EditPostComment;

internal sealed class EditPostCommentCommandHandler : ICommandHandler<EditPostCommentCommand>,
    IAsyncCommandHandler<EditPostCommentCommand>
{
    private readonly IAggregateRepository<Post, PostIdentifier> _aggregateRepository;

    public EditPostCommentCommandHandler(IAggregateRepository<Post, PostIdentifier> aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }

    public ICommandResult Handle(EditPostCommentCommand command)
    {
        PostComment postComment = null; // TODO: Get the aggregate

        postComment.Edit(PostCommentText.Create(command.Comment), _aggregateRepository, command.IdentityContext);

        // TODO: Save the aggregate

        return CommandResult.Success(postComment);
    }

    public async Task<ICommandResult> HandleAsync(EditPostCommentCommand command,
        CancellationToken cancellationToken = default)
    {
        PostComment postComment = null; // TODO: Get the aggregate

        postComment.Edit(PostCommentText.Create(command.Comment), _aggregateRepository, command.IdentityContext);

        // TODO: Save the aggregate

        return CommandResult.Success(postComment);
    }
}