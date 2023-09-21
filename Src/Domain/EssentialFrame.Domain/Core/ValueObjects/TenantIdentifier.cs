using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.ValueObjects;

public sealed class TenantIdentifier : TypedGuidIdentifier
{
    private TenantIdentifier(Guid value) : base(value)
    {
    }

    public static implicit operator TenantIdentifier(Guid identifier)
    {
        return New(identifier);
    }

    public static TenantIdentifier New(Guid value)
    {
        return new TenantIdentifier(value);
    }
}