namespace EssentialFrame.Domain.ValueObjects;

public class TypedGuidIdentifier : TypedIdentifierBase<Guid>
{
    protected TypedGuidIdentifier(Guid identifier) : base(identifier)
    {
    }

    public static implicit operator TypedGuidIdentifier(Guid identifier)
    {
        return identifier == Guid.Empty ? null : new TypedGuidIdentifier(identifier);
    }
    
    public override bool Empty()
    {
        return Identifier == Guid.Empty;
    }
}