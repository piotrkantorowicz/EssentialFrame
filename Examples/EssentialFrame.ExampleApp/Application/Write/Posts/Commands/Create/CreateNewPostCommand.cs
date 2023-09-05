using System;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.ExampleApp.Application.Write.Posts.Commands.Create.Dtos;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.Create;

public class CreateNewPostCommand : Command
{
    public CreateNewPostCommand(IIdentityContext identityContext, CreatePostDto post) : base(identityContext)
    {
        Post = post;
    }

    public CreateNewPostCommand(Guid aggregateIdentifier, IIdentityContext identityContext, CreatePostDto post) : base(
        aggregateIdentifier, identityContext)
    {
        Post = post;
    }

    public CreateNewPostCommand(Guid aggregateIdentifier, Guid commandIdentifier, IIdentityContext identityContext,
        CreatePostDto post) : base(aggregateIdentifier, commandIdentifier, identityContext)
    {
        Post = post;
    }

    public CreateNewPostCommand(Guid aggregateIdentifier, Guid commandIdentifier, int expectedVersion,
        IIdentityContext identityContext, CreatePostDto post) : base(aggregateIdentifier, commandIdentifier,
        expectedVersion, identityContext)
    {
        Post = post;
    }

    public CreatePostDto Post { get; set; }
}