using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.RemovePostComment;

internal sealed class RemovePostCommandHandler : ICommandHandler<RemovePostCommand>,
    IAsyncCommandHandler<RemovePostCommand>
{
    private readonly IEventSourcingAggregateRepository<Post, PostIdentifier> _aggregateRepository;

    public RemovePostCommandHandler(IEventSourcingAggregateRepository<Post, PostIdentifier> aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }

    public ICommandResult Handle(RemovePostCommand command)
    {
        PostComment postComment = null; // TODO: Get the aggregate

        postComment.Remove(DeletedReason.Create(command.Reason), _aggregateRepository, command.IdentityContext);

        // TODO: Save the aggregate

        return CommandResult.Success(postComment);
    }

    public async Task<ICommandResult> HandleAsync(RemovePostCommand command,
        CancellationToken cancellationToken = default)
    {
        PostComment postComment = null; // TODO: Get the aggregate

        postComment.Remove(DeletedReason.Create(command.Reason), _aggregateRepository, command.IdentityContext);

        // TODO: Save the aggregate

        return CommandResult.Success(postComment);
    }
}