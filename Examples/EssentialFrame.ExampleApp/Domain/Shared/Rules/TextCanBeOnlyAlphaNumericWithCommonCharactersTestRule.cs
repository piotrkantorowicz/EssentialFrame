﻿using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;
using EssentialFrame.Extensions;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Shared.Rules;

internal sealed class TextCanBeOnlyAlphaNumericWithCommonCharactersTestRule : BusinessRule
{
    private readonly string _text;

    public TextCanBeOnlyAlphaNumericWithCommonCharactersTestRule(Type domainObjectType, string text) : base(
        domainObjectType, BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _text = text;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value can be only alpha numeric with common special characters";

    public override bool IsBroken()
    {
        return _text?.IsAlphaNumericWithSpacesAndCommonCharacters() == false;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("Text", _text);
    }
}