using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names.Rules;

public class NameCannotBeEmptyRule : BusinessRule
{
    private readonly string _nameValue;

    public NameCannotBeEmptyRule(Type domainObjectType, string nameValue) : base(domainObjectType,
        BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _nameValue = nameValue;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value cannot be empty";

    public override bool IsBroken()
    {
        return string.IsNullOrEmpty(_nameValue);
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("NameValue", _nameValue);
    }
}