using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles.Rules;

public class TitleCanHaveAtLeast3AndAtMost250CharactersRule : BusinessRule
{
    private readonly string _titleValue;

    public TitleCanHaveAtLeast3AndAtMost250CharactersRule(Type domainObjectType, string titleValue) : base(
        domainObjectType, BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _titleValue = titleValue;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value have to be between 3 and 250 characters";

    public override bool IsBroken()
    {
        return _titleValue?.Length is < 3 or > 250;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("TitleValue", _titleValue);
    }
}