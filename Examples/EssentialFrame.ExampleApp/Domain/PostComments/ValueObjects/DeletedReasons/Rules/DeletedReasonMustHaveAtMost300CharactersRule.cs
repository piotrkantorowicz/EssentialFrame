using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons.Rules;

public class DeletedReasonMustHaveAtMost300CharactersRule : BusinessRule
{
    private readonly string _deletedReasonValue;

    public DeletedReasonMustHaveAtMost300CharactersRule(Type domainObjectType, string deletedReasonValue) : base(
        domainObjectType, BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _deletedReasonValue = deletedReasonValue;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value have to be between 3 and 1500 characters";

    public override bool IsBroken()
    {
        return _deletedReasonValue?.Length is < 3 or > 1500;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("DeletedReasonValue", _deletedReasonValue);
    }
}