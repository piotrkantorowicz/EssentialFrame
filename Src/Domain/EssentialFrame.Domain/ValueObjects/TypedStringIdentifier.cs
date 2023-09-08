namespace EssentialFrame.Domain.ValueObjects;

public abstract class TypedStringIdentifier : TypedIdentifierBase<string>
{
    protected TypedStringIdentifier(string identifier) : base(identifier)
    {
    }

    public override bool Empty()
    {
        return string.IsNullOrEmpty(Identifier);
    }
}