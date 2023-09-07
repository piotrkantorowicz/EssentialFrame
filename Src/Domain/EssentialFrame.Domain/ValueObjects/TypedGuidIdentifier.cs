namespace EssentialFrame.Domain.ValueObjects;

public class TypedGuidIdentifier : TypedIdentifierBase<Guid>
{
    protected TypedGuidIdentifier(Guid identifier) : base(identifier)
    {
    }

    public bool Empty()
    {
        return Identifier == Guid.Empty;
    }
}