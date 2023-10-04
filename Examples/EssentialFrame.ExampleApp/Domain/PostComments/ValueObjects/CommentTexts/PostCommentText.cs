using System.Collections.Generic;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.ExampleApp.Domain.Shared.Rules;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;

public sealed class PostCommentText : ValueObject
{
    private const int MinLenght = 3;
    private const int MaxLenght = 1500;

    private PostCommentText(string value)
    {
        CheckRule(new TextCanNotBeNullOrEmptyRule(GetType(), value));
        CheckRule(new TextCanBeOnlyAlphaNumericWithCommonCharactersTestRule(GetType(), value));
        CheckRule(new TextCanHaveSpecifiedNumberOfCharactersRule(GetType(), value, MinLenght, MaxLenght));

        Value = value;
    }

    public string Value { get; }

    public static PostCommentText Create(string value)
    {
        return new PostCommentText(value);
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