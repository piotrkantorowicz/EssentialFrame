using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.Domain.ValueObjects;

public abstract class ValueObject
{
    public override int GetHashCode()
    {
        return GetEqualityComponents().Aggregate(1, (current, obj) =>
        {
            unchecked
            {
                return (current * 23) + (obj?.GetHashCode() ?? 0);
            }
        });
    }
    
    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        ValueObject valueObject = (ValueObject)obj;

        return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
    }

    public static bool operator ==(ValueObject a, ValueObject b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
        {
            return true;
        }

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(ValueObject a, ValueObject b)
    {
        return !(a == b);
    }

    protected virtual void CheckRule(ValueObjectBusinessRuleBase rule, bool useExtraParameters = true)
    {
        if (rule.IsBroken())
        {
            if (useExtraParameters)
            {
                rule.AddExtraParameters();
            }

            throw new BusinessRuleValidationException(rule);
        }
    }

    protected abstract IEnumerable<object> GetEqualityComponents();
}