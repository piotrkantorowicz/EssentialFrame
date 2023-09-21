using System;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.InReplyTo;

public class InReplyToCommand : Command
{
    public InReplyToCommand(Guid aggregateIdentifier, IIdentityContext identityContext) : base(aggregateIdentifier,
        identityContext)
    {
    }

    public Guid PostIdentifier { get; }

    public Guid UserIdentifier { get; }

    public string Comment { get; }
}