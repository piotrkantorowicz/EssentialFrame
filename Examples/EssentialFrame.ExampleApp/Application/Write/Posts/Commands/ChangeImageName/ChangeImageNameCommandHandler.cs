using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ChangeImageName;

internal sealed class ChangeImageNameCommandHandler : ICommandHandler<ChangeImageNameCommand>,
    IAsyncCommandHandler<ChangeImageNameCommand>
{
    private readonly IAggregateRepository _aggregateRepository;

    public ChangeImageNameCommandHandler(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }
    
    public ICommandResult Handle(ChangeImageNameCommand command)
    {
        Post post = _aggregateRepository.Get<Post>(command.AggregateIdentifier, command.IdentityContext);

        post.ChangeImageName(command.ImageId, Name.Create(command.ImageName));
        _aggregateRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(ChangeImageNameCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = await _aggregateRepository.GetAsync<Post>(command.AggregateIdentifier, command.IdentityContext,
            cancellationToken);

        post.ChangeImageName(command.ImageId, Name.Create(command.ImageName));
        await _aggregateRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }
}
