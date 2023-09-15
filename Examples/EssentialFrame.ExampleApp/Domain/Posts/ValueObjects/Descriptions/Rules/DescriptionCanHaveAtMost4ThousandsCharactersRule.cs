using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions.Rules;

public class DescriptionCanHaveAtMost4ThousandsCharactersRule : BusinessRule
{
    private readonly string _descriptionValue;

    public DescriptionCanHaveAtMost4ThousandsCharactersRule(Type domainObjectType, string descriptionValue) : base(
        domainObjectType, BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _descriptionValue = descriptionValue;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value can have at most 4000 characters";

    public override bool IsBroken()
    {
        return _descriptionValue?.Length > 4000;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("DescriptionValue", _descriptionValue);
    }
}