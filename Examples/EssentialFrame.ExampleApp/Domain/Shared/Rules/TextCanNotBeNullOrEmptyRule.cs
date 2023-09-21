using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Shared.Rules;

internal sealed class TextCanNotBeNullOrEmptyRule : BusinessRule
{
    private readonly string _text;

    public TextCanNotBeNullOrEmptyRule(Type domainObjectType, string text) : base(domainObjectType,
        BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _text = text;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value cannot be empty";

    public override bool IsBroken()
    {
        return string.IsNullOrEmpty(_text);
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("Text", _text);
    }
}