using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Repositories;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.DeleteImages;

internal sealed class DeleteImagesCommandHandler : ICommandHandler<DeleteImagesCommand>,
    IAsyncCommandHandler<DeleteImagesCommand>
{
    private readonly IPostRepository _postRepository;

    public DeleteImagesCommandHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public ICommandResult Handle(DeleteImagesCommand command)
    {
        Post post = _postRepository.Get(PostIdentifier.New(command.AggregateIdentifier));

        post.DeleteImages(command.ImagesIds, command.IdentityContext);
        _postRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(DeleteImagesCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = await _postRepository.GetAsync(PostIdentifier.New(command.AggregateIdentifier),
            cancellationToken);

        post.DeleteImages(command.ImagesIds, command.IdentityContext);
        await _postRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }
}