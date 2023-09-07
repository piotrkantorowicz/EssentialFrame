using System;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Comments.Commands.AddPostComment;

public class AddPostCommentCommand : Command
{
    public AddPostCommentCommand(Guid aggregateIdentifier, IIdentityContext identityContext, Guid postIdentifier,
        Guid userIdentifier, Guid? replyToCommentIdentifier, string comment) : base(aggregateIdentifier,
        identityContext)
    {
        PostIdentifier = postIdentifier;
        UserIdentifier = userIdentifier;
        ReplyToCommentIdentifier = replyToCommentIdentifier;
        Comment = comment;
    }

    public Guid PostIdentifier { get; }

    public Guid UserIdentifier { get; }

    public Guid? ReplyToCommentIdentifier { get; }

    public string Comment { get; }
}