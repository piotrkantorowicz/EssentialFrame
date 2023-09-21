namespace EssentialFrame.Domain.Core.ValueObjects.Core;

public abstract class TypedIntIdentifier : TypedIdentifierBase<int>
{
    protected TypedIntIdentifier(int value) : base(value)
    {
    }

    public override bool IsEmpty()
    {
        return Value == 0;
    }
}