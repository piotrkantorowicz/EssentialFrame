using System;
using System.Collections.Generic;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.DeleteImages;

public class DeleteImagesCommand : Command
{
    public DeleteImagesCommand(Guid aggregateIdentifier, IIdentityContext identityContext, HashSet<Guid> imagesIds) :
        base(aggregateIdentifier, identityContext)
    {
        ImagesIds = imagesIds;
    }

    public DeleteImagesCommand(Guid aggregateIdentifier, Guid commandIdentifier, IIdentityContext identityContext,
        HashSet<Guid> imagesIds) : base(aggregateIdentifier, commandIdentifier, identityContext)
    {
        ImagesIds = imagesIds;
    }

    public DeleteImagesCommand(Guid aggregateIdentifier, Guid commandIdentifier, IIdentityContext identityContext,
        int expectedVersion, HashSet<Guid> imagesIds) : base(aggregateIdentifier, commandIdentifier, expectedVersion,
        identityContext)
    {
        ImagesIds = imagesIds;
    }

    public HashSet<Guid> ImagesIds { get; }
}