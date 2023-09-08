using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.AddPostComment;

internal sealed class AddPostCommentCommandHandler : ICommandHandler<AddPostCommentCommand>,
    IAsyncCommandHandler<AddPostCommentCommand>
{
    private readonly IAggregateRepository<Post, PostIdentifier> _aggregateRepository;

    public AddPostCommentCommandHandler(IAggregateRepository<Post, PostIdentifier> aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }

    public ICommandResult Handle(AddPostCommentCommand command)
    {
        PostComment postComment = PostComment.Create(PostCommentIdentifier.New(command.AggregateIdentifier),
            PostIdentifier.New(command.PostIdentifier), UserIdentifier.New(command.UserIdentifier),
            PostCommentIdentifier.New(command.ReplyToCommentIdentifier ?? Guid.Empty),
            PostCommentText.Create(command.Comment), command.IdentityContext, _aggregateRepository);

        // TODO: Save the aggregate

        return CommandResult.Success(postComment);
    }

    public async Task<ICommandResult> HandleAsync(AddPostCommentCommand command,
        CancellationToken cancellationToken = default)
    {
        PostComment postComment = PostComment.Create(PostCommentIdentifier.New(command.AggregateIdentifier),
            PostIdentifier.New(command.PostIdentifier), UserIdentifier.New(command.UserIdentifier),
            PostCommentIdentifier.New(command.ReplyToCommentIdentifier ?? Guid.Empty),
            PostCommentText.Create(command.Comment), command.IdentityContext, _aggregateRepository);

        // TODO: Save the aggregate

        return CommandResult.Success(postComment);
    }
}