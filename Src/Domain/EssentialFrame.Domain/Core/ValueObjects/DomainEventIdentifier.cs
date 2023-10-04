using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.ValueObjects;

public sealed class DomainEventIdentifier : TypedIdentifierBase<Guid>
{
    private DomainEventIdentifier(Guid value) : base(value)
    {
    }

    public static implicit operator DomainEventIdentifier(Guid identifier)
    {
        return New(identifier);
    }

    public static implicit operator Guid(DomainEventIdentifier identifier)
    {
        return identifier.Value;
    }

    public static DomainEventIdentifier New(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Value cannot be empty", nameof(value));
        }

        return new DomainEventIdentifier(value);
    }

    public override bool IsEmpty()
    {
        return Value == Guid.Empty;
    }
}