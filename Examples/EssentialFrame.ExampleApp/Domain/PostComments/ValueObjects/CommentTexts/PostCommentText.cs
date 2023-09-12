using System.Collections.Generic;
using EssentialFrame.Domain.ValueObjects.Core;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts.Rules;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.CommentTexts;

public sealed class PostCommentText : ValueObject
{
    private PostCommentText(string value)
    {
        CheckRule(new PostCommentTextCanBeOnlyAlphaNumericWithCommonCharactersTestRule(GetType(), value));
        CheckRule(new PostCommentTextCanHaveAtLeast3AndAtMost1500CharactersRule(GetType(), value));

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