namespace EssentialFrame.Domain.Core.ValueObjects.Core;

public abstract class TypedStringIdentifier : TypedIdentifierBase<string>
{
    protected TypedStringIdentifier(string value) : base(value)
    {
    }

    public override bool Empty()
    {
        return string.IsNullOrEmpty(Value);
    }
}