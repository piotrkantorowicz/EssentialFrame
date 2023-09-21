using System;
using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

public sealed class PostIdentifier : TypedGuidIdentifier
{
    private PostIdentifier(Guid value) : base(value)
    {
    }

    public static implicit operator PostIdentifier(Guid identifier)
    {
        return New(identifier);
    }

    public static PostIdentifier New(Guid value)
    {
        return new PostIdentifier(value);
    }
}