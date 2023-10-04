using System;
using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

public sealed class PostCommentIdentifier : TypedIdentifierBase<Guid>
{
    private PostCommentIdentifier(Guid value) : base(value)
    {
    }

    public static implicit operator PostCommentIdentifier(Guid identifier)
    {
        return New(identifier);
    }

    public static implicit operator Guid(PostCommentIdentifier identifier)
    {
        return identifier.Value;
    }
    
    public static PostCommentIdentifier New()
    {
        return new PostCommentIdentifier(Guid.NewGuid());
    }

    public static PostCommentIdentifier New(Guid value)
    { 
        return new PostCommentIdentifier(value);
    }

    public static PostCommentIdentifier Empty()
    {
        return new PostCommentIdentifier(Guid.Empty);
    }

    public override bool IsEmpty()
    {
        return Value == Guid.Empty;
    }
}