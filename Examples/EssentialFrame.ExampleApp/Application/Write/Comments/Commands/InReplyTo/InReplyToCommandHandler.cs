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

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.InReplyTo;

internal sealed class InReplyToCommandHandler : ICommandHandler<InReplyToCommand>,
    IAsyncCommandHandler<InReplyToCommand>
{
    private readonly IPostCommentDomainService _postCommentDomainService;

    public InReplyToCommandHandler(IPostCommentDomainService postCommentDomainService)
    {
        _postCommentDomainService = postCommentDomainService ??
                                    throw new ArgumentNullException(nameof(postCommentDomainService));
    }

    public ICommandResult Handle(InReplyToCommand command)
    {
        PostComment postComment = _postCommentDomainService.InReplyTo(
            PostCommentIdentifier.New(command.AggregateIdentifier), PostCommentText.Create(command.Comment),
            DomainIdentity.New(command.IdentityContext));

        return CommandResult.Success(postComment);
    }

    public async Task<ICommandResult> HandleAsync(InReplyToCommand command,
        CancellationToken cancellationToken = default)
    {
        PostComment postComment = await _postCommentDomainService.InReplyToAsync(
            PostCommentIdentifier.New(command.AggregateIdentifier), PostCommentText.Create(command.Comment),
            DomainIdentity.New(command.IdentityContext), cancellationToken);

        return CommandResult.Success(postComment);
    }
}