using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.PostComments.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.RemovePostComment;

internal sealed class RemovePostCommandHandler : ICommandHandler<RemovePostCommand>,
    IAsyncCommandHandler<RemovePostCommand>
{
    private readonly IAggregateRepository _aggregateRepository;

    public RemovePostCommandHandler(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }

    public ICommandResult Handle(RemovePostCommand command)
    {
        PostComment postComment = null; // TODO: Get the aggregate

        postComment.Remove(DeletedReason.Create(command.Reason), _aggregateRepository);

        // TODO: Save the aggregate

        return CommandResult.Success(postComment);
    }

    public async Task<ICommandResult> HandleAsync(RemovePostCommand command,
        CancellationToken cancellationToken = default)
    {
        PostComment postComment = null; // TODO: Get the aggregate

        postComment.Remove(DeletedReason.Create(command.Reason), _aggregateRepository);

        // TODO: Save the aggregate

        return CommandResult.Success(postComment);
    }
}