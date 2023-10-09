using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.RemovePostComment;

internal sealed class RemovePostCommandHandler : ICommandHandler<RemovePostCommand>,
    IAsyncCommandHandler<RemovePostCommand>
{
    private readonly IPostCommentDomainService _postCommentDomainService;

    public RemovePostCommandHandler(IPostCommentDomainService postCommentDomainService)
    {
        _postCommentDomainService = postCommentDomainService ??
                                    throw new ArgumentNullException(nameof(postCommentDomainService));
    }

    public ICommandResult Handle(RemovePostCommand command)
    {
        PostComment postComment = _postCommentDomainService.Remove(
            PostCommentIdentifier.New(command.AggregateIdentifier), DeletedReason.Create(command.Reason),
            command.IdentityContext);

        return CommandResult.Success(postComment);
    }

    public async Task<ICommandResult> HandleAsync(RemovePostCommand command, CancellationToken cancellationToken)
    {
        PostComment postComment = await _postCommentDomainService.RemoveAsync(
            PostCommentIdentifier.New(command.AggregateIdentifier), DeletedReason.Create(command.Reason),
            command.IdentityContext, cancellationToken);

        return CommandResult.Success(postComment);
    }
}