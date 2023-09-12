using EssentialFrame.Domain.ValueObjects.Core;

namespace EssentialFrame.Domain.ValueObjects;

public sealed class TenantIdentifier : TypedGuidIdentifier
{
    private TenantIdentifier(Guid value) : base(value)
    {
    }

    public static TenantIdentifier New()
    {
        return new TenantIdentifier(Guid.NewGuid());
    }

    public static TenantIdentifier New(Guid value)
    {
        return new TenantIdentifier(value);
    }
}