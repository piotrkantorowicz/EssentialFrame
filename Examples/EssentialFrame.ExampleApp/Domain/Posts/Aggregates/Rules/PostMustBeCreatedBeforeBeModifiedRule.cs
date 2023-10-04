using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.Posts.Aggregates.Rules;

public sealed class PostMustBeCreatedBeforeBeModifiedRule : IdentifiableBusinessRule<PostIdentifier, Guid>
{
    private readonly bool _isCreated;

    public PostMustBeCreatedBeforeBeModifiedRule(PostIdentifier domainObjectIdentifier, Type businessObjectType,
        bool isCreated) :
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