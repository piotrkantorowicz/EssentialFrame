using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ChangeDescription;

internal sealed class ChangeDescriptionCommandHandler : ICommandHandler<ChangeDescriptionCommand>,
    IAsyncCommandHandler<ChangeDescriptionCommand>
{
    private readonly IAggregateRepository<Post, PostIdentifier> _aggregateRepository;

    public ChangeDescriptionCommandHandler(IAggregateRepository<Post, PostIdentifier> aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }
    
    public ICommandResult Handle(ChangeDescriptionCommand command)
    {
        Post post = _aggregateRepository.Get(PostIdentifier.New(command.AggregateIdentifier));

        post.ChangeDescription(Description.Create(command.Description), command.IdentityContext);
        _aggregateRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(ChangeDescriptionCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = await _aggregateRepository.GetAsync(PostIdentifier.New(command.AggregateIdentifier),
            cancellationToken);

        post.ChangeDescription(Description.Create(command.Description), command.IdentityContext);
        await _aggregateRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }
}
