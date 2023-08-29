using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names.Rules;

public class NameCanHaveAtLeast3AndAtMost150CharactersRule : BusinessRule
{
    private readonly string _nameValue;

    public NameCanHaveAtLeast3AndAtMost150CharactersRule(Type domainObjectType, string nameValue) : base(
        domainObjectType, BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _nameValue = nameValue;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}, because value have to be between 3 and 150 characters";

    public override bool IsBroken()
    {
        return _nameValue?.Length is < 3 or > 150;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("NameValue", _nameValue);
    }
}