using System;
using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

public sealed class PostCommentIdentifier : TypedGuidIdentifier
{
    private PostCommentIdentifier(Guid identifier) : base(identifier)
    {
    }

    public static PostCommentIdentifier New()
    {
        return new PostCommentIdentifier(Guid.NewGuid());
    }

    public static PostCommentIdentifier New(Guid identifier)
    {
        return new PostCommentIdentifier(identifier);
    }
}