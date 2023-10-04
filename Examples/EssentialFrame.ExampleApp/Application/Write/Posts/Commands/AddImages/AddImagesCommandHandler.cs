using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.Repositories;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.AddImages;

internal sealed class AddImagesCommandHandler : ICommandHandler<AddImagesCommand>,
    IAsyncCommandHandler<AddImagesCommand>
{
    private readonly IPostRepository _postRepository;

    public AddImagesCommandHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public ICommandResult Handle(AddImagesCommand command)
    {
        Post post = _postRepository.Get(PostIdentifier.New(command.AggregateIdentifier));

        post.AddImages(command.Images.Select(i => Image.Create(Name.Create(i.ImageName), BytesContent.Create(i.Bytes)))
            .ToHashSet(), command.IdentityContext);

        _postRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(AddImagesCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = await _postRepository.GetAsync(PostIdentifier.New(command.AggregateIdentifier),
            cancellationToken);

        post.AddImages(command.Images.Select(i => Image.Create(Name.Create(i.ImageName), BytesContent.Create(i.Bytes)))
            .ToHashSet(), command.IdentityContext);

        await _postRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }
}