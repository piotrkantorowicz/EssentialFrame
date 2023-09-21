using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.PostComments.Shared.Rules;

internal sealed class TextCanHaveSpecifiedNumberOfCharactersRule : BusinessRule
{
    private readonly string _text;
    private readonly int _minLength;
    private readonly int _maxLength;

    public TextCanHaveSpecifiedNumberOfCharactersRule(Type domainObjectType, string text, int minLength = 0,
        int maxLength = 0) : base(domainObjectType, BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _text = text;
        _minLength = minLength;
        _maxLength = maxLength;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value have to be between 3 and 1500 characters";

    public override bool IsBroken()
    {
        return _text?.Length < _minLength && _text?.Length > _maxLength;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("Text", _maxLength);
    }
}