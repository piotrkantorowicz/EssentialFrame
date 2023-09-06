using System;
using System.Collections.Generic;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.ExampleApp.Application.Write.Posts.Commands.Create.Dtos;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.Create;

public class CreateNewPostCommand : Command
{
    public CreateNewPostCommand(IIdentityContext identityContext, string title, string description,
        DateTimeOffset expiration, HashSet<ImageDto> images) : base(identityContext)
    {
        Title = title;
        Description = description;
        Expiration = expiration;
        Images = images;
    }

    public CreateNewPostCommand(Guid aggregateIdentifier, IIdentityContext identityContext, string title,
        string description, DateTimeOffset expiration, HashSet<ImageDto> images) : base(aggregateIdentifier,
        identityContext)
    {
        Title = title;
        Description = description;
        Expiration = expiration;
        Images = images;
    }

    public CreateNewPostCommand(Guid aggregateIdentifier, Guid commandIdentifier, IIdentityContext identityContext,
        string title, string description, DateTimeOffset expiration, HashSet<ImageDto> images) : base(
        aggregateIdentifier, commandIdentifier, identityContext)
    {
        Title = title;
        Description = description;
        Expiration = expiration;
        Images = images;
    }

    public CreateNewPostCommand(Guid aggregateIdentifier, Guid commandIdentifier, int expectedVersion,
        IIdentityContext identityContext, string title, string description, DateTimeOffset expiration,
        HashSet<ImageDto> images) : base(aggregateIdentifier, commandIdentifier, expectedVersion, identityContext)
    {
        Title = title;
        Description = description;
        Expiration = expiration;
        Images = images;
    }

    public string Title { get; }

    public string Description { get; }

    public DateTimeOffset Expiration { get; }

    public HashSet<ImageDto> Images { get; }
}