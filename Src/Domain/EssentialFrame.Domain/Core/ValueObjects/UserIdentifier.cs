using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.ValueObjects;

public sealed class UserIdentifier : TypedIdentifierBase<Guid>
{
    private UserIdentifier(Guid value) : base(value)
    {
    }

    public static implicit operator UserIdentifier(Guid identifier)
    {
        return New(identifier);
    }

    public static implicit operator Guid(UserIdentifier identifier)
    {
        return identifier.Value;
    }

    public static UserIdentifier New(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Value cannot be empty", nameof(value));
        }

        return new UserIdentifier(value);
    }

    public override bool IsEmpty()
    {
        return Value == Guid.Empty;
    }
}