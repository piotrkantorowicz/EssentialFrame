namespace EssentialFrame.Domain.Core.ValueObjects.Core;

public class TypedGuidIdentifier : TypedIdentifierBase<Guid>
{
    protected TypedGuidIdentifier(Guid value) : base(value)
    {
    }
    
    public override bool Empty()
    {
        return Value == Guid.Empty;
    }
}