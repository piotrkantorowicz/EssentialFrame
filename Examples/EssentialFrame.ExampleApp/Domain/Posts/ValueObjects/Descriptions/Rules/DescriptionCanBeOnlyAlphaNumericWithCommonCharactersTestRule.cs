using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;
using EssentialFrame.Extensions;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions.Rules;

public class DescriptionCanBeOnlyAlphaNumericWithCommonCharactersTestRule : BusinessRule
{
    private readonly string _descriptionValue;

    public DescriptionCanBeOnlyAlphaNumericWithCommonCharactersTestRule(Type domainObjectType, string descriptionValue)
        : base(domainObjectType, BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _descriptionValue = descriptionValue;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value can be only alpha numeric with common special characters";

    public override bool IsBroken()
    {
        return _descriptionValue?.IsAlphaNumericWithSpacesAndCommonCharacters() == false;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("DescriptionValue", _descriptionValue);
    }
}