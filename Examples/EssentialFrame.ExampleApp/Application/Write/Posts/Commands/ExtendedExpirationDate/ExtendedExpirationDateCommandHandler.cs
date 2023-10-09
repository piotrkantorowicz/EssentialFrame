using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Repositories;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ExtendedExpirationDate;

internal sealed class ExtendedExpirationDateCommandHandler : ICommandHandler<ExtendedExpirationDateCommand>,
    IAsyncCommandHandler<ExtendedExpirationDateCommand>
{
    private readonly IPostRepository _postRepository;

    public ExtendedExpirationDateCommandHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public ICommandResult Handle(ExtendedExpirationDateCommand command)
    {
        Post post = _postRepository.Get(PostIdentifier.New(command.AggregateIdentifier));

        post.ExtendExpirationDate(Date.Create(command.ExpirationDate), DomainIdentity.New(command.IdentityContext));
        _postRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(ExtendedExpirationDateCommand command,
        CancellationToken cancellationToken)
    {
        Post post = await _postRepository.GetAsync(PostIdentifier.New(command.AggregateIdentifier),
            cancellationToken);

        post.ExtendExpirationDate(Date.Create(command.ExpirationDate), DomainIdentity.New(command.IdentityContext));
        await _postRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }
}