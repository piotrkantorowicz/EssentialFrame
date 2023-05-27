using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;
using EssentialFrame.TestData.Domain.Aggregates;
using EssentialFrame.Time;

namespace EssentialFrame.TestData.Domain.Rules;

public sealed class ExpiredTestAggregateCannotBeUpdatedRule : AggregateBusinessRuleBase
{
    private readonly DateTimeOffset _expiration;

    public ExpiredTestAggregateCannotBeUpdatedRule(Guid aggregateIdentifier, Type aggregateType,
        DateTimeOffset expiration) : base(aggregateIdentifier, aggregateType)
    {
        _expiration = expiration;
    }

    public override string Message =>
        $"This aggregate ({AggregateType.FullName}) with identifier ({AggregateIdentifier}) has been already expired. Expiration date time ({_expiration})";

    public override bool IsBroken()
    {
        return _expiration != SystemClock.Min && _expiration <= SystemClock.UtcNow;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd(nameof(TestAggregateState.Expiration), _expiration);
    }
}