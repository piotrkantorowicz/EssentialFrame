using System;
using System.Collections.Generic;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Application.Write.Posts.Commands.Delete;

public class DeleteCommand : Command
{
    public DeleteCommand(Guid aggregateIdentifier, IdentityContext identityContext,
        HashSet<PostCommentIdentifier> postCommentIdentifiers) : base(aggregateIdentifier, identityContext)
    {
        PostCommentIdentifiers = postCommentIdentifiers;
    }

    public HashSet<PostCommentIdentifier> PostCommentIdentifiers { get; }
}