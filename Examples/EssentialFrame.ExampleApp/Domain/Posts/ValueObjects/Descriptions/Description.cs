using System.Collections.Generic;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions.Rules;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;

public sealed class Description : ValueObject
{
    private Description(string value)
    {
        CheckRule(new DescriptionCanBeOnlyAlphaNumericWithCommonCharactersTestRule(GetType(), value));
        CheckRule(new DescriptionCanHaveAtMost4ThousandsCharactersRule(GetType(), value));

        Value = value;
    }

    public string Value { get; }

    public static Description Create(string value)
    {
        return new Description(value);
    }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(Value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}