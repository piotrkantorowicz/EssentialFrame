namespace EssentialFrame.Domain.ValueObjects;

public abstract class TypedIdentifierBase<T> : ValueObject
{
    protected TypedIdentifierBase(T identifier)
    {
        Identifier = identifier;
    }

    public T Identifier { get; }

    public abstract bool Empty();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Identifier;
    }
}