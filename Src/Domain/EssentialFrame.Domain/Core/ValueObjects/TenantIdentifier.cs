using EssentialFrame.Domain.Core.ValueObjects.Core;

namespace EssentialFrame.Domain.Core.ValueObjects;

public sealed class TenantIdentifier : TypedIdentifierBase<Guid>
{
    private TenantIdentifier(Guid value) : base(value)
    {
    }

    public static implicit operator TenantIdentifier(Guid identifier)
    {
        return New(identifier);
    }

    public static implicit operator Guid(TenantIdentifier identifier)
    {
        return identifier.Value;
    }

    public static TenantIdentifier New(Guid value)
    {
        return new TenantIdentifier(value);
    }

    public override bool IsEmpty()
    {
        return Value == Guid.Empty;
    }
}