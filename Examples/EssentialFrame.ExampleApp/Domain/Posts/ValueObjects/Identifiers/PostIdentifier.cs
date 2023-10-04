using System;
using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

public sealed class PostIdentifier : TypedIdentifierBase<Guid>
{
    private PostIdentifier(Guid value) : base(value)
    {
    }

    public static implicit operator PostIdentifier(Guid identifier)
    {
        return New(identifier);
    }

    public static implicit operator Guid(PostIdentifier identifier)
    {
        return identifier.Value;
    }

    public static PostIdentifier New(Guid value)
    {
        return new PostIdentifier(value);
    }

    public override bool IsEmpty()
    {
        return Value == Guid.Empty;
    }
}