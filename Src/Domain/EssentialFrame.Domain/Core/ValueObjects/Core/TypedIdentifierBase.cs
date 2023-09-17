using System.Reflection;
using EssentialFrame.Domain.Exceptions;

namespace EssentialFrame.Domain.Core.ValueObjects.Core;

public abstract class TypedIdentifierBase<T> : ValueObject
{
    protected TypedIdentifierBase(T value)
    {
        Value = value;
    }

    public T Value { get; }

    public abstract bool Empty();

    public override string ToString()
    {
        return Value.ToString();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static TAggregateIdentifier New<TAggregateIdentifier>(Guid value)
        where TAggregateIdentifier : TypedIdentifierBase<T>
    {
        ConstructorInfo[] constructors =
            typeof(TAggregateIdentifier).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (ConstructorInfo constructor in constructors)
        {
            List<string> parameterNames = constructor.GetParameters().Select(p => p.Name).ToList();

            if (parameterNames.Contains(nameof(value)))
            {
                return (TAggregateIdentifier)constructor.Invoke(new object[] { value });
            }
        }

        throw new MissingConstructorException(typeof(TAggregateIdentifier));
    }
}