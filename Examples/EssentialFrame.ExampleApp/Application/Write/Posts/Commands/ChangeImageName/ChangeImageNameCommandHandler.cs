using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ChangeImageName;

internal sealed class ChangeImageNameCommandHandler : ICommandHandler<ChangeImageNameCommand>,
    IAsyncCommandHandler<ChangeImageNameCommand>
{
    private readonly IEventSourcingAggregateRepository<Post, PostIdentifier> _aggregateRepository;

    public ChangeImageNameCommandHandler(IEventSourcingAggregateRepository<Post, PostIdentifier> aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }
    
    public ICommandResult Handle(ChangeImageNameCommand command)
    {
        Post post = _aggregateRepository.Get(PostIdentifier.New(command.AggregateIdentifier));

        post.ChangeImageName(command.ImageId, Name.Create(command.ImageName), command.IdentityContext);
        _aggregateRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(ChangeImageNameCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = await _aggregateRepository.GetAsync(PostIdentifier.New(command.AggregateIdentifier),
            cancellationToken);

        post.ChangeImageName(command.ImageId, Name.Create(command.ImageName), command.IdentityContext);
        await _aggregateRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }
}
