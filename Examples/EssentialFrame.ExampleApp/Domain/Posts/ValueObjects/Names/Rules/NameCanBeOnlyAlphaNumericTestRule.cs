using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;
using EssentialFrame.Extensions;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names.Rules;

public class NameCanBeOnlyAlphaNumericTestRule : BusinessRule
{
    private readonly string _nameValue;

    public NameCanBeOnlyAlphaNumericTestRule(Type domainObjectType, string nameValue) : base(domainObjectType,
        BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _nameValue = nameValue;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}, because value can be only alpha numeric";

    public override bool IsBroken()
    {
        return _nameValue?.IsAlphaNumericWithSpaces() == false;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("NameValue", _nameValue);
    }
}