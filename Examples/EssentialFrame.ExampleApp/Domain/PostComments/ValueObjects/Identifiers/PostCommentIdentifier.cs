using System;
using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

public sealed class PostCommentIdentifier : TypedGuidIdentifier
{
    private PostCommentIdentifier(Guid value) : base(value)
    {
    }
    
    public static PostCommentIdentifier New()
    {
        return new PostCommentIdentifier(Guid.NewGuid());
    }

    public static PostCommentIdentifier New(Guid value)
    {
        return new PostCommentIdentifier(value);
    }
}