using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.TestData.Domain.Rules;

public sealed class TitleValueCannotBeEmptyRule : ValueObjectBusinessRuleBase
{
    private readonly string _titleValue;

    public TitleValueCannotBeEmptyRule(Type valueObjectType, string titleValue) : base(valueObjectType)
    {
        _titleValue = titleValue;
    }

    public override string Message =>
        $"Unable to set value for value object ({ValueObjectType.FullName}), because value cannot be empty.";

    public override bool IsBroken()
    {
        return string.IsNullOrEmpty(_titleValue);
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("Value", _titleValue);
    }
}