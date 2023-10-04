using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;

public class PostDeletedDomainEvent : DomainEvent<PostIdentifier, Guid>
{
    public PostDeletedDomainEvent(PostIdentifier aggregateIdentifier, DomainIdentity identityContext,
        IReadOnlyCollection<PostCommentIdentifier> postCommentIdentifiers) : base(aggregateIdentifier, identityContext)
    {
        PostCommentIdentifiers = postCommentIdentifiers;
    }

    public IReadOnlyCollection<PostCommentIdentifier> PostCommentIdentifiers { get; }
}