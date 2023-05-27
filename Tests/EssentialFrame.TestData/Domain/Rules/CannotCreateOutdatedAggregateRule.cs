using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;
using EssentialFrame.TestData.Domain.Aggregates;
using EssentialFrame.Time;

namespace EssentialFrame.TestData.Domain.Rules;

public class CannotCreateOutdatedAggregateRule : AggregateBusinessRuleBase
{
    private readonly DateTimeOffset _expiration;

    public CannotCreateOutdatedAggregateRule(Guid aggregateIdentifier, Type aggregateType, DateTimeOffset expiration) :
        base(aggregateIdentifier, aggregateType)
    {
        _expiration = expiration;
    }

    public override string Message =>
        $"Cannot create an aggregate ({AggregateType.FullName}) with identifier ({AggregateIdentifier}) with outdated " +
        $"expiration date: {_expiration}.";

    public override bool IsBroken()
    {
        return _expiration < SystemClock.UtcNow;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd(nameof(TestAggregateState.Expiration), _expiration);
    }
}