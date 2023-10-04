using System;
using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

public sealed class AuthorIdentifier : TypedIdentifierBase<Guid>
{
    private AuthorIdentifier(Guid value) : base(value)
    {
    }

    public static implicit operator AuthorIdentifier(Guid identifier)
    {
        return New(identifier);
    }

    public static implicit operator Guid(AuthorIdentifier identifier)
    {
        return identifier.Value;
    }

    public static AuthorIdentifier New(Guid value)
    {
        return new AuthorIdentifier(value);
    }

    public override bool IsEmpty()
    {
        return Value == Guid.Empty;
    }
}