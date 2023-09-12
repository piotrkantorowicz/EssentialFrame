using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Time;

namespace EssentialFrame.ExampleApp.Domain.Posts.Aggregates.Rules;

public class CannotCreateOutdatedPostRule : IdentifiableBusinessRule<PostIdentifier>
{
    private readonly Date _expiration;

    public CannotCreateOutdatedPostRule(PostIdentifier domainObjectIdentifier, Type businessObjectType, Date expiration)
        : base(
        domainObjectIdentifier, businessObjectType, BusinessRuleTypes.AggregateBusinessRule)
    {
        _expiration = expiration;
    }

    public override string Message =>
        $"Cannot create ({DomainObjectType.FullName}) with identifier ({DomainObjectIdentifier}) with outdated expiration date: {_expiration.Value}";

    public override bool IsBroken()
    {
        return _expiration?.Value < SystemClock.UtcNow;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd("Expiration", _expiration);
    }
}