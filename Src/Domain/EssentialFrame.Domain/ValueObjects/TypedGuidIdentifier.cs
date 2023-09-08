namespace EssentialFrame.Domain.ValueObjects;

public abstract class TypedGuidIdentifier : TypedIdentifierBase<Guid>
{
    protected TypedGuidIdentifier(Guid identifier) : base(identifier)
    {
    }

    public override bool Empty()
    {
        return Identifier == Guid.Empty;
    }

    public static implicit operator Guid(TypedGuidIdentifier typedGuidIdentifier)
    {
        return typedGuidIdentifier.Identifier;
    }
}