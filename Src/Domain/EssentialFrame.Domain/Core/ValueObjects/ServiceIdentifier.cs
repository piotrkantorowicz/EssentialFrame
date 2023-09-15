using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.ValueObjects;

public sealed class ServiceIdentifier : TypedStringIdentifier
{
    private ServiceIdentifier(string value) : base(value)
    {
    }

    public static ServiceIdentifier New(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("Value cannot be empty", nameof(value));
        }

        return new ServiceIdentifier(value);
    }
}