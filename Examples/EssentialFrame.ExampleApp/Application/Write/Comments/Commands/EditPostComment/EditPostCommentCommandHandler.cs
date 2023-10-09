using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.EditPostComment;

internal sealed class EditPostCommentCommandHandler : ICommandHandler<EditPostCommentCommand>,
    IAsyncCommandHandler<EditPostCommentCommand>
{
    private readonly IPostCommentDomainService _postCommentDomainService;

    public EditPostCommentCommandHandler(IPostCommentDomainService postCommentDomainService)
    {
        _postCommentDomainService = postCommentDomainService ??
                                    throw new ArgumentNullException(nameof(postCommentDomainService));
    }

    public ICommandResult Handle(EditPostCommentCommand command)
    {
        PostComment postComment = _postCommentDomainService.Edit(PostCommentIdentifier.New(command.AggregateIdentifier),
            PostCommentText.Create(command.Comment), DomainIdentity.New(command.IdentityContext));

        return CommandResult.Success(postComment);
    }

    public async Task<ICommandResult> HandleAsync(EditPostCommentCommand command, CancellationToken cancellationToken)
    {
        PostComment postComment = await _postCommentDomainService.EditAsync(
            PostCommentIdentifier.New(command.AggregateIdentifier), PostCommentText.Create(command.Comment),
            DomainIdentity.New(command.IdentityContext), cancellationToken);

        return CommandResult.Success(postComment);
    }
}