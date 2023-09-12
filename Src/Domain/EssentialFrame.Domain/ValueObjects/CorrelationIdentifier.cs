using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.ValueObjects;

public sealed class CorrelationIdentifier : TypedGuidIdentifier
{
    private CorrelationIdentifier(Guid value) : base(value)
    {
    }

    public static CorrelationIdentifier New()
    {
        return new CorrelationIdentifier(Guid.NewGuid());
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