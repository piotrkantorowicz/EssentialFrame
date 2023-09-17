using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;
using EssentialFrame.Extensions;

namespace EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.DeletedReasons.Rules;

public class DeletedReasonCanBeOnlyAlphaNumericWithCommonCharactersTestRule : BusinessRule
{
    private readonly string _deletedReasonValue;

    public DeletedReasonCanBeOnlyAlphaNumericWithCommonCharactersTestRule(Type domainObjectType,
        string deletedReasonValue) : base(domainObjectType, BusinessRuleTypes.ValueObjectBusinessRule)
    {
        _deletedReasonValue = deletedReasonValue;
    }

    public override string Message =>
        $"Unable to set value for ({DomainObjectType.FullName}), because value can be only alpha numeric with common special characters";

    public override bool IsBroken()
    {
        return _deletedReasonValue?.IsAlphaNumericWithSpacesAndCommonCharacters() == false;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("DeletedReasonValue", _deletedReasonValue);
    }
}