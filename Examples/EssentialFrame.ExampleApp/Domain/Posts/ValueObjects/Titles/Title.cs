using System.Collections.Generic;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.ExampleApp.Domain.Shared.Rules;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;

public sealed class Title : ValueObject
{
    private const int MinLenght = 3;
    private const int MaxLenght = 250;
    
    private Title(string value)
    {
        CheckRule(new TextCanNotBeNullOrEmptyRule(GetType(), value));
        CheckRule(new TextCanBeOnlyAlphaNumericTestRule(GetType(), value));

        CheckRule(new TextCanHaveSpecifiedNumberOfCharactersRule(GetType(), value, MinLenght, MaxLenght));
        
        Value = value;
    }

    public string Value { get; }

    public static Title Default(string value)
    {
        return new Title(value);
    }

    public static Title Uppercase(string value)
    {
        return new Title(value.ToUpperInvariant());
    }

    public static Title Lowercase(string value)
    {
        return new Title(value.ToLowerInvariant());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}