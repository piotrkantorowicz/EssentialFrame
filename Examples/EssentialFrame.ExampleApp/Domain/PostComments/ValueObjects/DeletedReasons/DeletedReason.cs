using System.Collections.Generic;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.ExampleApp.Domain.Shared.Rules;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons;

public sealed class DeletedReason : ValueObject
{
    private const int MaxLenght = 300;
    
    private DeletedReason(string value)
    {
        CheckRule(new TextCanBeOnlyAlphaNumericWithCommonCharactersTestRule(GetType(), value));
        CheckRule(new TextCanHaveSpecifiedNumberOfCharactersRule(GetType(), value, maxLength: MaxLenght));

        Value = value;
    }

    public string Value { get; }

    public static DeletedReason Create(string value)
    {
        return new DeletedReason(value);
    }

    public static DeletedReason PostDeleted(PredefinedDeletedReasons predefinedDeletedReasons)
    {
        return new DeletedReason(predefinedDeletedReasons.DisplayName);
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