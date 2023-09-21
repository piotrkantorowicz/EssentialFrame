using System.Collections.Generic;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.ExampleApp.Domain.PostComments.Shared.Rules;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;

public sealed class Description : ValueObject
{
    private const int MaxLenght = 4000;
    
    private Description(string value)
    {
        CheckRule(new TextCanBeOnlyAlphaNumericWithCommonCharactersTestRule(GetType(), value));
        CheckRule(new TextCanHaveSpecifiedNumberOfCharactersRule(GetType(), value, maxLength: MaxLenght));

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