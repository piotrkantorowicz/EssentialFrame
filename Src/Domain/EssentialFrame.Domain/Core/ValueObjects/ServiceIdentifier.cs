using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.ValueObjects;

public sealed class ServiceIdentifier : TypedIdentifierBase<string>
{
    private ServiceIdentifier(string value) : base(value)
    {
    }

    public static implicit operator ServiceIdentifier(string identifier)
    {
        return New(identifier);
    }

    public static implicit operator string(ServiceIdentifier identifier)
    {
        return identifier.Value;
    }
    
    public static ServiceIdentifier New(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be empty", nameof(value));
        }

        return new ServiceIdentifier(value);
    }

    public override bool IsEmpty()
    {
        return string.IsNullOrEmpty(Value);
    }
}