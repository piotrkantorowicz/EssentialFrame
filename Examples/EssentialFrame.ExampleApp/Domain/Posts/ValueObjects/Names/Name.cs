using System.Collections.Generic;
using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names.Rules;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;

public sealed class Name : ValueObject
{
    private Name(string value)
    {
        CheckRule(new NameCannotBeEmptyRule(GetType(), value));
        CheckRule(new NameCanBeOnlyAlphaNumericTestRule(GetType(), value));
        CheckRule(new NameCanHaveAtLeast3AndAtMost150CharactersRule(GetType(), value));

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