using System;
using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

public sealed class AuthorIdentifier : TypedGuidIdentifier
{
    private AuthorIdentifier(Guid value) : base(value)
    {
    }

    public static implicit operator AuthorIdentifier(Guid identifier)
    {
        return New(identifier);
    }

    public static AuthorIdentifier New(Guid value)
    {
        return new AuthorIdentifier(value);
    }
}