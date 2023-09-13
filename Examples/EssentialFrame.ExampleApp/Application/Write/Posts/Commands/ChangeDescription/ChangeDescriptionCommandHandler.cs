using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ChangeDescription;

internal sealed class ChangeDescriptionCommandHandler : ICommandHandler<ChangeDescriptionCommand>,
    IAsyncCommandHandler<ChangeDescriptionCommand>
{
    private readonly IEventSourcingAggregateRepository<Post, PostIdentifier> _eventSourcingAggregateRepository;

    public ChangeDescriptionCommandHandler(
        IEventSourcingAggregateRepository<Post, PostIdentifier> eventSourcingAggregateRepository)
    {
        _eventSourcingAggregateRepository = eventSourcingAggregateRepository ??
                                            throw new ArgumentNullException(nameof(eventSourcingAggregateRepository));
    }
    
    public ICommandResult Handle(ChangeDescriptionCommand command)
    {
        Post post = _eventSourcingAggregateRepository.Get(PostIdentifier.New(command.AggregateIdentifier));

        post.ChangeDescription(Description.Create(command.Description), command.IdentityContext);
        _eventSourcingAggregateRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(ChangeDescriptionCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = await _eventSourcingAggregateRepository.GetAsync(PostIdentifier.New(command.AggregateIdentifier),
            cancellationToken);

        post.ChangeDescription(Description.Create(command.Description), command.IdentityContext);
        await _eventSourcingAggregateRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }
}
