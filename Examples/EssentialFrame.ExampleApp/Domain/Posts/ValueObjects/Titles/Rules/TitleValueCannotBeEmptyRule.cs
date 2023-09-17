using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles.Rules;

public sealed class TitleValueCannotBeEmptyRule : BusinessRule
{
    private readonly string _titleValue;

    public TitleValueCannotBeEmptyRule(Type domainObjectType, string titleValue) : base(domainObjectType,
        BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _titleValue = titleValue;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value cannot be empty";

    public override bool IsBroken()
    {
        return string.IsNullOrEmpty(_titleValue);
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("TitleValue", _titleValue);
    }
}