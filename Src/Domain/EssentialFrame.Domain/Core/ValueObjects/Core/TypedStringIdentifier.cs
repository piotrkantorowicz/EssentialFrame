namespace EssentialFrame.Domain.Core.ValueObjects.Core;

public abstract class TypedStringIdentifier : TypedIdentifierBase<string>
{
    protected TypedStringIdentifier(string value) : base(value)
    {
    }

    public static implicit operator string(TypedStringIdentifier typedStringIdentifier)
    {
        return typedStringIdentifier.Value;
    }
    
    public override bool IsEmpty()
    {
        return string.IsNullOrEmpty(Value);
    }
}