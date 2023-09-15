using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ChangeTitle;

internal sealed class ChangeTitleCommandHandler : ICommandHandler<ChangeTitleCommand>,
    IAsyncCommandHandler<ChangeTitleCommand>
{
    private readonly IEventSourcingAggregateRepository<Post, PostIdentifier> _postRepository;

    public ChangeTitleCommandHandler(IEventSourcingAggregateRepository<Post, PostIdentifier> postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }
    
    public ICommandResult Handle(ChangeTitleCommand command)
    {
        Post post = _postRepository.Get(PostIdentifier.New(command.AggregateIdentifier));

        post.ChangeTitle(Title.Default(command.Title), command.IdentityContext);
        _postRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(ChangeTitleCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = await _postRepository.GetAsync(PostIdentifier.New(command.AggregateIdentifier),
            cancellationToken);

        post.ChangeTitle(Title.Default(command.Title), command.IdentityContext);
        await _postRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }
}