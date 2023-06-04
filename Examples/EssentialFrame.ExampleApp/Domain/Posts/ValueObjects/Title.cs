using System.Collections.Generic;
using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.ExampleApp.Domain.Posts.Rules;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects;

public sealed class Title : ValueObject
{
    private Title(string value, bool uppercase)
    {
        CheckRule(new TitleValueCannotBeEmptyRule(GetType(), value));

        Value = value;
        Uppercase = uppercase;
    }

    public string Value { get; }

    public bool Uppercase { get; }

    public static Title Create(string value, bool uppercase)
    {
        return new Title(value, uppercase);
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