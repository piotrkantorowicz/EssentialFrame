using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ChangeTitle;

internal sealed class ChangeTitleCommandHandler : ICommandHandler<ChangeTitleCommand>,
    IAsyncCommandHandler<ChangeTitleCommand>
{
    private readonly IAggregateRepository _aggregateRepository;

    public ChangeTitleCommandHandler(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }
    
    public ICommandResult Handle(ChangeTitleCommand command)
    {
        Post post = _aggregateRepository.Get<Post>(command.AggregateIdentifier);

        post.ChangeTitle(Title.Default(command.Title), command.IdentityContext);
        _aggregateRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(ChangeTitleCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = await _aggregateRepository.GetAsync<Post>(command.AggregateIdentifier, cancellationToken);

        post.ChangeTitle(Title.Default(command.Title), command.IdentityContext);
        await _aggregateRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }
}