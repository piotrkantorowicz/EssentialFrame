using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.Time;

namespace EssentialFrame.ExampleApp.Domain.Posts.Rules;

public class CannotCreateOutdatedPostRule : AggregateBusinessRuleBase
{
    private readonly DateTimeOffset _expiration;

    public CannotCreateOutdatedPostRule(Guid aggregateIdentifier, Type aggregateType, DateTimeOffset expiration) : base(
        aggregateIdentifier, aggregateType)
    {
        _expiration = expiration;
    }

    public override string Message =>
        $"Cannot create ({AggregateType.FullName}) with identifier ({AggregateIdentifier}) with outdated " +
        $"expiration date: {_expiration}.";

    public override bool IsBroken()
    {
        return _expiration < SystemClock.UtcNow;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd(nameof(PostState.Expiration), _expiration);
    }
}