using System;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.Create;

public class CreateNewPostCommand : Command
{
    public CreateNewPostCommand(Guid aggregateIdentifier, IIdentityContext identityContext) : base(aggregateIdentifier,
        identityContext)
    {
    }

    public CreateNewPostCommand(Guid aggregateIdentifier, Guid commandIdentifier, IIdentityContext identityContext) :
        base(aggregateIdentifier, commandIdentifier, identityContext)
    {
    }

    public CreateNewPostCommand(Guid aggregateIdentifier, Guid commandIdentifier, int expectedVersion,
        IIdentityContext identityContext) : base(aggregateIdentifier, commandIdentifier, expectedVersion,
        identityContext)
    {
    }
}