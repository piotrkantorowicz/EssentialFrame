﻿using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.DeleteImages;

internal sealed class DeleteImagesCommandHandler : ICommandHandler<DeleteImagesCommand>,
    IAsyncCommandHandler<DeleteImagesCommand>
{
    private readonly IEventSourcingAggregateRepository<Post, PostIdentifier> _aggregateRepository;

    public DeleteImagesCommandHandler(IEventSourcingAggregateRepository<Post, PostIdentifier> aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }

    public ICommandResult Handle(DeleteImagesCommand command)
    {
        Post post = _aggregateRepository.Get(PostIdentifier.New(command.AggregateIdentifier));

        post.DeleteImages(command.ImagesIds, command.IdentityContext);
        _aggregateRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(DeleteImagesCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = await _aggregateRepository.GetAsync(PostIdentifier.New(command.AggregateIdentifier),
            cancellationToken);

        post.DeleteImages(command.ImagesIds, command.IdentityContext);
        await _aggregateRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }
}