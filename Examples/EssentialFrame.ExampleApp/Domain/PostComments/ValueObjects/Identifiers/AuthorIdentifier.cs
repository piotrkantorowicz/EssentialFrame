using System;
using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

public sealed class AuthorIdentifier : TypedGuidIdentifier
{
    private AuthorIdentifier(Guid value) : base(value)
    {
    }

    public static AuthorIdentifier New()
    {
        return new AuthorIdentifier(Guid.NewGuid());
    }

    public static AuthorIdentifier New(Guid value)
    {
        return new AuthorIdentifier(value);
    }
}