using System;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ChangeDescription;

public class ChangeDescriptionCommand : Command
{
    public ChangeDescriptionCommand(Guid aggregateIdentifier, IdentityContext identityContext, string description) :
        base(aggregateIdentifier, identityContext)
    {
        Description = description;
    }

    public ChangeDescriptionCommand(Guid aggregateIdentifier, Guid commandIdentifier, IdentityContext identityContext,
        string description) : base(aggregateIdentifier, commandIdentifier, identityContext)
    {
        Description = description;
    }

    public ChangeDescriptionCommand(Guid aggregateIdentifier, Guid commandIdentifier, IdentityContext identityContext,
        int expectedVersion, string description) : base(aggregateIdentifier, commandIdentifier, expectedVersion,
        identityContext)
    {
        Description = description;
    }

    public string Description { get; }
}