using System.Collections.Generic;
using EssentialFrame.Domain.ValueObjects;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons.Rules;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;

public sealed class DeletedReason : ValueObject
{
    private DeletedReason(string value)
    {
        CheckRule(new DeletedReasonCanBeOnlyAlphaNumericWithCommonCharactersTestRule(GetType(), value));
        CheckRule(new DeletedReasonMustHaveAtMost300CharactersRule(GetType(), value));

        Value = value;
    }

    public string Value { get; }

    public static DeletedReason Create(string value)
    {
        return new DeletedReason(value);
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