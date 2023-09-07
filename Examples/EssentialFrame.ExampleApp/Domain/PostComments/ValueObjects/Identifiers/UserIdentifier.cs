using System;
using EssentialFrame.Domain.ValueObjects;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;

public sealed class UserIdentifier : TypedGuidIdentifier
{
    private UserIdentifier(Guid id) : base(id)
    {
    }

    public static UserIdentifier New()
    {
        return new UserIdentifier(Guid.NewGuid());
    }

    public static UserIdentifier New(Guid identifier)
    {
        return new UserIdentifier(identifier);
    }
}