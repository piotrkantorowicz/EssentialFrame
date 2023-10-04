using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.EventSourcing.Core.Factories;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.Repositories;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.Create;

internal sealed class CreateNewPostCommandHandler : ICommandHandler<CreateNewPostCommand>,
    IAsyncCommandHandler<CreateNewPostCommand>
{
    private readonly IPostRepository _postRepository;

    public CreateNewPostCommandHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public ICommandResult Handle(CreateNewPostCommand command)
    {
        Post post = Create(command);

        _postRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(CreateNewPostCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = Create(command);

        await _postRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }

    private static Post Create(CreateNewPostCommand command)
    {
        Post post = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            PostIdentifier.New(command.AggregateIdentifier));

        post.Create(Title.Default(command.Title), Description.Create(command.Description),
            Date.Create(command.Expiration), command.Images
                .Select(i => Image.Create(Name.Create(i.ImageName), BytesContent.Create(i.Bytes))).ToHashSet(),
            command.IdentityContext);

        return post;
    }
}