using System;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Repositories;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ChangeImageName;

internal sealed class ChangeImageNameCommandHandler : ICommandHandler<ChangeImageNameCommand>,
    IAsyncCommandHandler<ChangeImageNameCommand>
{
    private readonly IPostRepository _postRepository;

    public ChangeImageNameCommandHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }
    
    public ICommandResult Handle(ChangeImageNameCommand command)
    {
        Post post = _postRepository.Get(PostIdentifier.New(command.AggregateIdentifier));

        post.ChangeImageName(command.ImageId, Name.Create(command.ImageName), command.IdentityContext);
        _postRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(ChangeImageNameCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = await _postRepository.GetAsync(PostIdentifier.New(command.AggregateIdentifier),
            cancellationToken);

        post.ChangeImageName(command.ImageId, Name.Create(command.ImageName), command.IdentityContext);
        await _postRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }
}
