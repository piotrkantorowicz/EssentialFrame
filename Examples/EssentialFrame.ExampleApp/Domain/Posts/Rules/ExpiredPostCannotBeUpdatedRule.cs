using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules.Base;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.Time;

namespace EssentialFrame.ExampleApp.Domain.Posts.Rules;

public sealed class ExpiredPostCannotBeUpdatedRule : AggregateBusinessRuleBase
{
    private readonly DateTimeOffset _expiration;

    public ExpiredPostCannotBeUpdatedRule(Guid aggregateIdentifier, Type aggregateType, DateTimeOffset expiration) :
        base(aggregateIdentifier, aggregateType)
    {
        _expiration = expiration;
    }

    public override string Message =>
        $"This ({AggregateType.FullName}) with identifier ({AggregateIdentifier}) has been already expired. Expiration date time ({_expiration})";

    public override bool IsBroken()
    {
        return _expiration != SystemClock.Min && _expiration <= SystemClock.UtcNow;
    }

    public override void AddExtraParameters()
    {
        Parameters.TryAdd(nameof(PostState.Expiration), _expiration);
    }
}