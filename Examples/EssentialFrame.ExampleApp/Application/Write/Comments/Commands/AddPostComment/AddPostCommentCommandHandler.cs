using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.AddPostComment;

internal sealed class AddPostCommentCommandHandler : ICommandHandler<AddPostCommentCommand>,
    IAsyncCommandHandler<AddPostCommentCommand>
{
    private readonly IPostCommentDomainService _postCommentDomainService;

    public AddPostCommentCommandHandler(IPostCommentDomainService postCommentDomainService)
    {
        _postCommentDomainService = postCommentDomainService ??
                                    throw new ArgumentNullException(nameof(postCommentDomainService));
    }

    public ICommandResult Handle(AddPostCommentCommand command)
    {
        PostComment postComment = _postCommentDomainService.CreateNew(
            PostCommentIdentifier.New(command.AggregateIdentifier), PostIdentifier.New(command.PostIdentifier),
            PostCommentText.Create(command.Comment), command.IdentityContext);

        return CommandResult.Success(postComment);
    }

    public async Task<ICommandResult> HandleAsync(AddPostCommentCommand command, CancellationToken cancellationToken)
    {
        PostComment postComment = await _postCommentDomainService.CreateNewAsync(
            PostCommentIdentifier.New(command.AggregateIdentifier), PostIdentifier.New(command.PostIdentifier),
            PostCommentText.Create(command.Comment), command.IdentityContext, cancellationToken);

        return CommandResult.Success(postComment);
    }
}