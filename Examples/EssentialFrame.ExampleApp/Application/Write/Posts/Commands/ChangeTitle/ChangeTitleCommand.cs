using System;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.ChangeTitle;

public class ChangeTitleCommand : Command
{
    public ChangeTitleCommand(Guid aggregateIdentifier, IIdentityContext identityContext, string title, bool uppercase)
        : base(aggregateIdentifier, identityContext)
    {
        Title = title;
        Uppercase = uppercase;
    }

    public ChangeTitleCommand(Guid aggregateIdentifier, Guid commandIdentifier, IIdentityContext identityContext,
        string title, bool uppercase) : base(aggregateIdentifier, commandIdentifier, identityContext)
    {
        Title = title;
        Uppercase = uppercase;
    }

    public ChangeTitleCommand(Guid aggregateIdentifier, Guid commandIdentifier, IIdentityContext identityContext,
        int expectedVersion, string title, bool uppercase) : base(aggregateIdentifier, commandIdentifier,
        expectedVersion, identityContext)
    {
        Title = title;
        Uppercase = uppercase;
    }

    public string Title { get; }

    public bool Uppercase { get; }
}