using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Domain.Events.Core.Factories;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.Create;

internal sealed class CreateNewPostCommandHandler : ICommandHandler<CreateNewPostCommand>,
    IAsyncCommandHandler<CreateNewPostCommand>
{
    private readonly IAggregateRepository _aggregateRepository;

    public CreateNewPostCommandHandler(IAggregateRepository aggregateRepository)
    {
        _aggregateRepository = aggregateRepository ?? throw new ArgumentNullException(nameof(aggregateRepository));
    }

    public ICommandResult Handle(CreateNewPostCommand command)
    {
        Post post = Create(command);

        _aggregateRepository.Save(post);

        return CommandResult.Success(post.State);
    }

    public async Task<ICommandResult> HandleAsync(CreateNewPostCommand command,
        CancellationToken cancellationToken = default)
    {
        Post post = Create(command);

        await _aggregateRepository.SaveAsync(post, cancellationToken: cancellationToken);

        return CommandResult.Success(post.State);
    }

    private static Post Create(CreateNewPostCommand command)
    {
        Post post = GenericAggregateFactory<Post>.CreateAggregate(command.AggregateIdentifier, command.IdentityContext);

        post.Create(Title.Default(command.Post.Title), Description.Create(command.Post.Description),
            Date.Create(command.Post.Expiration),
            command.Post.Images.Select(i => Image.Create(Name.Create(i.ImageName), BytesContent.Create(i.Bytes)))
                .ToHashSet());

        return post;
    }
}