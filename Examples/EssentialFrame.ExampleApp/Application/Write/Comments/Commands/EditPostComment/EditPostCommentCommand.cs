using System;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.EditPostComment;

public class EditPostCommentCommand : Command
{
    public EditPostCommentCommand(Guid aggregateIdentifier, IIdentityContext identityContext, Guid postIdentifier,
        string comment) : base(aggregateIdentifier, identityContext)
    {
        PostIdentifier = postIdentifier;
        Comment = comment;
    }

    public Guid PostIdentifier { get; }

    public string Comment { get; }
}