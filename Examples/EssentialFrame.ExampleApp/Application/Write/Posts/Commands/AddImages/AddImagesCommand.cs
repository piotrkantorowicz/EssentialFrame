using System;
using System.Collections.Generic;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.ExampleApp.Application.Write.Posts.Dtos;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.AddImages;

public class AddImagesCommand : Command
{
    public AddImagesCommand(Guid aggregateIdentifier, IIdentityContext identityContext, HashSet<ImageDto> images) :
        base(aggregateIdentifier, identityContext)
    {
        Images = images;
    }

    public AddImagesCommand(Guid aggregateIdentifier, Guid commandIdentifier, IIdentityContext identityContext,
        HashSet<ImageDto> images) : base(aggregateIdentifier, commandIdentifier, identityContext)
    {
        Images = images;
    }

    public AddImagesCommand(Guid aggregateIdentifier, Guid commandIdentifier, IIdentityContext identityContext,
        int expectedVersion, HashSet<ImageDto> images) : base(aggregateIdentifier, commandIdentifier, expectedVersion,
        identityContext)
    {
        Images = images;
    }

    public HashSet<ImageDto> Images { get; }
}