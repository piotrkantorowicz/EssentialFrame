using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ExtendedExpirationDate;

internal sealed class ExtendedExpirationDateCommandHandler : ICommandHandler<ExtendedExpirationDateCommand>,
    IAsyncCommandHandler<ExtendedExpirationDateCommand>
{
    private readonly IAggregateRepository<Post, PostIdentifier> _aggregateRepository;

    public ExtendedExpirationDateCommandHandler(IAggregateRepository<Post, PostIdentifier> aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }

    public ICommandResult Handle(ExtendedExpirationDateCommand command)
    {
        Post post = _aggregateRepository.Get(PostIdentifier.New(command.AggregateIdentifier));

        post.ExtendExpirationDate(Date.Create(command.ExpirationDate), command.IdentityContext);
        _aggregateRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(ExtendedExpirationDateCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = await _aggregateRepository.GetAsync(PostIdentifier.New(command.AggregateIdentifier),
            cancellationToken);

        post.ExtendExpirationDate(Date.Create(command.ExpirationDate), command.IdentityContext);
        await _aggregateRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }
}