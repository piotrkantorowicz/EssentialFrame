using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.ValueObjects;

public sealed class CorrelationIdentifier : TypedGuidIdentifier
{
    private CorrelationIdentifier(Guid value) : base(value)
    {
    }

    public static implicit operator CorrelationIdentifier(Guid identifier)
    {
        return New(identifier);
    }

    public static CorrelationIdentifier New(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Value cannot be empty", nameof(value));
        }

        return new CorrelationIdentifier(value);
    }
}