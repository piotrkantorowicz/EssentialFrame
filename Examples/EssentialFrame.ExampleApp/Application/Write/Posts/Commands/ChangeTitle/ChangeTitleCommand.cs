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

    public string Title { get; }

    public bool Uppercase { get; }
}