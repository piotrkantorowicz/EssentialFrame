using System;
using System.Collections.Generic;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.DeleteImages;

public class DeleteImagesCommand : Command
{
    public DeleteImagesCommand(Guid aggregateIdentifier, IdentityContext identityContext, HashSet<Guid> imagesIds) :
        base(aggregateIdentifier, identityContext)
    {
        ImagesIds = imagesIds;
    }

    public HashSet<Guid> ImagesIds { get; }
}