using System.Collections.Generic;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.ExampleApp.Domain.Shared.Rules;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;

public sealed class Name : ValueObject
{
    private const int MinLenght = 3;
    private const int MaxLenght = 150;

    private Name(string value)
    {
        CheckRule(new TextCanNotBeNullOrEmptyRule(GetType(), value));
        CheckRule(new TextCanBeOnlyAlphaNumericTestRule(GetType(), value));

        CheckRule(new TextCanHaveSpecifiedNumberOfCharactersRule(GetType(), value, MinLenght, MaxLenght));

        Value = value;
    }

    public string Value { get; }

    public static Name Create(string value)
    {
        return new Name(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}