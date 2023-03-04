using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules;
using EssentialFrame.TestData.Domain.Aggregates;
using EssentialFrame.Time;

namespace EssentialFrame.TestData.Domain.Rules;

public sealed class ExpiredTestAggregateCannotBeUpdatedRule : BaseBusinessRule
{
    private readonly DateTimeOffset _expiration;

    public ExpiredTestAggregateCannotBeUpdatedRule(Guid aggregateIdentifier, Type aggregateType,
        DateTimeOffset expiration) : base(aggregateIdentifier, aggregateType)
    {
        _expiration = expiration;
    }

    public override string Message =>
        $"This aggregate ({_aggregateType.FullName}) with identifier ({_aggregateIdentifier}) has been already expired. Expiration date time ({_expiration})";

    public override bool IsBroken()
    {
        return _expiration < SystemClock.Now;
    }

    protected override void AddExtraParameters()
    {
        Parameters.TryAdd(nameof(TestAggregateState.Expiration), _expiration);
    }
}