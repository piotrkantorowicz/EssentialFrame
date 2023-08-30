namespace EssentialFrame.Domain.ValueObjects;

public class TypedIdBase<T> : ValueObject
{
    protected TypedIdBase(T id)
    {
        Id = id;
    }

    public T Id { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
    }
}