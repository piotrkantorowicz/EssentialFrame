namespace EssentialFrame.Domain.ValueObjects;

public class TypedIdentifierBase<T> : ValueObject
{
    protected TypedIdentifierBase(T identifier)
    {
        Identifier = identifier;
    }

    public T Identifier { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Identifier;
    }
}