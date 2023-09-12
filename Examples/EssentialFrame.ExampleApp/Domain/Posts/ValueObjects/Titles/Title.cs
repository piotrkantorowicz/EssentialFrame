using System.Collections.Generic;
using EssentialFrame.Domain.ValueObjects.Core;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles.Rules;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;

public sealed class Title : ValueObject
{
    private Title(string value)
    {
        CheckRule(new TitleValueCannotBeEmptyRule(GetType(), value));
        CheckRule(new TitleCanBeOnlyAlphaNumericWithCommonCharactersTestRule(GetType(), value));
        CheckRule(new TitleCanHaveAtLeast3AndAtMost250CharactersRule(GetType(), value));

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