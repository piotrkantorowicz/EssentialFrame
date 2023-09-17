using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Rules.Base;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Time;

namespace EssentialFrame.ExampleApp.Domain.Posts.Aggregates.Rules;

public sealed class ExpiredPostCannotBeUpdatedRule : IdentifiableBusinessRule<PostIdentifier>
{
    private readonly Date _expiration;

    public ExpiredPostCannotBeUpdatedRule(PostIdentifier domainObjectIdentifier, Type businessObjectType,
        Date expiration) : base(
        domainObjectIdentifier, businessObjectType, BusinessRuleTypes.AggregateBusinessRule)
    {
        _expiration = expiration;
    }

    public override string Message =>
        $"This ({DomainObjectType.FullName}) with identifier ({DomainObjectIdentifier}) has been already expired. Expiration date time ({_expiration})";

    public override bool IsBroken()
    {
        return _expiration?.Value != SystemClock.Min && _expiration?.Value <= SystemClock.UtcNow;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("Expiration", _expiration);
    }
}