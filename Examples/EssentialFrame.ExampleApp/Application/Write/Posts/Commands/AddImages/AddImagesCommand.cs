using System;
using System.Collections.Generic;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.ExampleApp.Application.Write.Posts.Commands.AddImages.Dtos;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.AddImages;

public class AddImagesCommand : Command
{
    public AddImagesCommand(Guid aggregateIdentifier, IdentityContext identityContext, HashSet<ImageDto> images) :
        base(aggregateIdentifier, identityContext)
    {
        Images = images;
    }

    public HashSet<ImageDto> Images { get; }
}