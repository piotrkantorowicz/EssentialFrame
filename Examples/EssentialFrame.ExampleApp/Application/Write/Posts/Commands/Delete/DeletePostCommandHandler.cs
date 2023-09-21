using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.Delete;

internal sealed class DeletePostCommandHandler : ICommandHandler<DeleteCommand>, IAsyncCommandHandler<DeleteCommand>
{
    private readonly IEventSourcingAggregateRepository<Post, PostIdentifier> _postRepository;

    public DeletePostCommandHandler(IEventSourcingAggregateRepository<Post, PostIdentifier> postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public ICommandResult Handle(DeleteCommand command)
    {
        Post post = _postRepository.Get(PostIdentifier.New(command.AggregateIdentifier));

        post.Delete(command.PostCommentIdentifiers, command.IdentityContext);
        post.SafeDelete();

        _postRepository.Save(post);
        _postRepository.Box(post.AggregateIdentifier);

        return CommandResult.Success();
    }

    public async Task<ICommandResult> HandleAsync(DeleteCommand command, CancellationToken cancellationToken = default)
    {
        Post post = await _postRepository.GetAsync(PostIdentifier.New(command.AggregateIdentifier), cancellationToken);

        post.Delete(command.PostCommentIdentifiers, command.IdentityContext);
        post.SafeDelete();

        await _postRepository.SaveAsync(post, cancellationToken: cancellationToken);
        await _postRepository.BoxAsync(post.AggregateIdentifier, cancellationToken);

        return CommandResult.Success();
    }
}