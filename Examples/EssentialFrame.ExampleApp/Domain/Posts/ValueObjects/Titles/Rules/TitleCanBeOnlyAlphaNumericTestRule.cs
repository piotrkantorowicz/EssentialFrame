using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;
using EssentialFrame.Extensions;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles.Rules;

public class TitleCanBeOnlyAlphaNumericWithCommonCharactersTestRule : BusinessRule
{
    private readonly string _titleValue;

    public TitleCanBeOnlyAlphaNumericWithCommonCharactersTestRule(Type domainObjectType, string titleValue) : base(
        domainObjectType, BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _titleValue = titleValue;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}, because value can be only alpha numeric with common special characters";

    public override bool IsBroken()
    {
        return _titleValue?.IsAlphaNumericWithSpacesAndCommonCharacters() == false;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("TitleValue", _titleValue);
    }
}