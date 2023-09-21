namespace EssentialFrame.Domain.Core.ValueObjects.Core;

public abstract class TypedGuidIdentifier : TypedIdentifierBase<Guid>
{
    protected TypedGuidIdentifier(Guid value) : base(value)
    {
    }

    public static implicit operator Guid(TypedGuidIdentifier typedGuidIdentifier)
    {
        return typedGuidIdentifier.Value;
    }

    public override bool IsEmpty()
    {
        return Value == Guid.Empty;
    }
}