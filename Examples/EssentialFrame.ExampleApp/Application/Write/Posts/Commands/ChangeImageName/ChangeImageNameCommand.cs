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

    public Guid ImageId { get; }

    public string ImageName { get; }
}