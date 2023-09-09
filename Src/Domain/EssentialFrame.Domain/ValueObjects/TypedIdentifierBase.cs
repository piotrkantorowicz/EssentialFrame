using System.Reflection;
using EssentialFrame.Domain.Exceptions;

namespace EssentialFrame.Domain.ValueObjects;

public abstract class TypedIdentifierBase<T> : ValueObject
{
    protected TypedIdentifierBase(T identifier)
    {
        Identifier = identifier;
    }

    public T Identifier { get; }

    public abstract bool Empty();

    public override string ToString()
    {
        return Identifier.ToString();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Identifier;
    }

    public static TAggregateIdentifier New<TAggregateIdentifier>(Guid identifier)
        where TAggregateIdentifier : TypedIdentifierBase<T>
    {
        ConstructorInfo[] constructors =
            typeof(TAggregateIdentifier).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (ConstructorInfo constructor in constructors)
        {
            List<string> parameterNames = constructor.GetParameters().Select(p => p.Name).ToList();

            if (parameterNames.Contains("id"))
            {
                return (TAggregateIdentifier)constructor.Invoke(new object[] { identifier });
            }
        }

        throw new MissingConstructorException(typeof(TAggregateIdentifier));
    }
}