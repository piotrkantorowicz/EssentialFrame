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

    public static implicit operator string(TypedIdentifierBase<T> identifier)
    {
        return identifier.ToString();
    }
    
    public override string ToString()
    {
        return Value.ToString();
    }

    public abstract bool IsEmpty();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static TAggregateIdentifier New<TAggregateIdentifier>(object value)
        where TAggregateIdentifier : TypedIdentifierBase<T> 
    {
        object typedObject = Parse(value);
        
        ConstructorInfo[] constructors =
            typeof(TAggregateIdentifier).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (ConstructorInfo constructor in constructors)
        {
            List<string> parameterNames = constructor.GetParameters().Select(p => p.Name).ToList();

            if (parameterNames.Contains(nameof(value)))
            {
                return (TAggregateIdentifier)constructor.Invoke(new[] { typedObject });
            }
        }

        throw new MissingConstructorException(typeof(TAggregateIdentifier));
    }

    private static object Parse(object value)
    {
        if (int.TryParse(value.ToString(), out int intValue))
        {
            return intValue;
        }

        if (Guid.TryParse(value.ToString(), out Guid guidValue))
        {
            return guidValue;
        }

        if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
        {
            return stringValue;
        }

        throw new InvalidCastException($"Unable to cast string value to any matching typed id. Value: {value}");
    }
}