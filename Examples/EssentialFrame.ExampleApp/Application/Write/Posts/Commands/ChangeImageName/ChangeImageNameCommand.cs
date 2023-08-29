using System;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ChangeImageName;

public class ChangeImageNameCommand : Command
{
    public ChangeImageNameCommand(Guid aggregateIdentifier, IIdentityContext identityContext, Guid imageId,
        string imageName) : base(aggregateIdentifier, identityContext)
    {
        ImageId = imageId;
        ImageName = imageName;
    }

    public ChangeImageNameCommand(Guid aggregateIdentifier, Guid commandIdentifier, IIdentityContext identityContext,
        Guid imageId, string imageName) : base(aggregateIdentifier, commandIdentifier, identityContext)
    {
        ImageId = imageId;
        ImageName = imageName;
    }

    public ChangeImageNameCommand(Guid aggregateIdentifier, Guid commandIdentifier, IIdentityContext identityContext,
        int expectedVersion, Guid imageId, string imageName) : base(aggregateIdentifier, commandIdentifier,
        expectedVersion, identityContext)
    {
        ImageId = imageId;
        ImageName = imageName;
    }

    public Guid ImageId { get; }

    public string ImageName { get; }
}