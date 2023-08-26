using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;

namespace EssentialFrame.ExampleApp.Domain.Posts.Aggregates.Rules;

public sealed class PostMustBeCreatedBeforeBeModifiedRule : BusinessRule
{
    private readonly bool _isCreated;

    public PostMustBeCreatedBeforeBeModifiedRule(Guid domainObjectIdentifier, Type businessObjectType, bool isCreated) :
        base(domainObjectIdentifier, businessObjectType, BusinessRuleTypes.AggregateBusinessRule)
    {
        _isCreated = isCreated;
    }

    public override string Message =>
        $" ({DomainObjectType.FullName}) with identifier ({DomainObjectIdentifier}) must be created before be modified";

    public override bool IsBroken()
    {
        return !_isCreated;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("IsCreated", _isCreated);
    }
}