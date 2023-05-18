using System.Collections.Generic;
using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.TestData.Domain.Rules;

namespace EssentialFrame.TestData.Domain.ValueObjects;

public sealed class TestTitle : ValueObject
{
    private TestTitle(string value, bool uppercase)
    {
        CheckRule(new TitleValueCannotBeEmptyRule(GetType(), value));

        Value = value;
        Uppercase = uppercase;
    }

    public string Value { get; }

    public bool Uppercase { get; }

    public static TestTitle Create(string value, bool uppercase)
    {
        return new TestTitle(value, uppercase);
    }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(Value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Uppercase;
    }
}