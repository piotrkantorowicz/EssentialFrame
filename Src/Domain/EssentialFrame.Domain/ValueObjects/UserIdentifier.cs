using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.ValueObjects;

public sealed class UserIdentifier : TypedGuidIdentifier
{
    private UserIdentifier(Guid value) : base(value)
    {
    }

    public static UserIdentifier New(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Value cannot be empty", nameof(value));
        }

        return new UserIdentifier(value);
    }
}