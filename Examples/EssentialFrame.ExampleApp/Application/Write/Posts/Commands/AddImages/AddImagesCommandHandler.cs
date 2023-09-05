using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.AddImages;

internal sealed class AddImagesCommandHandler : ICommandHandler<AddImagesCommand>,
    IAsyncCommandHandler<AddImagesCommand>
{
    private readonly IAggregateRepository _aggregateRepository;

    public AddImagesCommandHandler(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }

    public ICommandResult Handle(AddImagesCommand command)
    {
        Post post = _aggregateRepository.Get<Post>(command.AggregateIdentifier);

        post.AddImages(command.Images.Select(i => Image.Create(Name.Create(i.ImageName), BytesContent.Create(i.Bytes)))
            .ToHashSet());

        _aggregateRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(AddImagesCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = await _aggregateRepository.GetAsync<Post>(command.AggregateIdentifier, cancellationToken);

        post.AddImages(command.Images.Select(i => Image.Create(Name.Create(i.ImageName), BytesContent.Create(i.Bytes)))
            .ToHashSet());

        await _aggregateRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }
}