namespace EssentialFrame.Domain.ValueObjects;

public abstract class TypedIntIdentifier : TypedIdentifierBase<int>
{
    protected TypedIntIdentifier(int identifier) : base(identifier)
    {
    }

    public override bool Empty()
    {
        return Identifier == 0;
    }
}