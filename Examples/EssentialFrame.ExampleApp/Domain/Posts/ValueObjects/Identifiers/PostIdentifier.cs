using System;
using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

public sealed class PostIdentifier : TypedGuidIdentifier
{
    private PostIdentifier(Guid id) : base(id)
    {
    }

    public static PostIdentifier New()
    {
        return new PostIdentifier(Guid.NewGuid());
    }

    public static PostIdentifier New(Guid identifier)
    {
        return new PostIdentifier(identifier);
    }
}