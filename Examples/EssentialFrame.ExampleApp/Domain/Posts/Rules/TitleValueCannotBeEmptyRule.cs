using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.Posts.Rules;

public sealed class TitleValueCannotBeEmptyRule : ValueObjectBusinessRuleBase
{
    private readonly string _titleValue;

    public TitleValueCannotBeEmptyRule(Type valueObjectType, string titleValue) : base(valueObjectType)
    {
        _titleValue = titleValue;
    }

    public override string Message =>
        $"Unable to set value for ({ValueObjectType.FullName}), because value cannot be empty.";

    public override bool IsBroken()
    {
        return string.IsNullOrEmpty(_titleValue);
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("Value", _titleValue);
    }
}