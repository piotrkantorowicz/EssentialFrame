using System;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.RemovePostComment;

public class RemovePostCommand : Command
{
    public RemovePostCommand(Guid aggregateIdentifier, IdentityContext identityContext, string reason) : base(
        aggregateIdentifier, identityContext)
    {
        Reason = reason;
    }

    public string Reason { get; }
}