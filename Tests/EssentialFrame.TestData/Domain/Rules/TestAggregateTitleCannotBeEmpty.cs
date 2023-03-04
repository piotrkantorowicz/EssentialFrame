using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Rules;
using EssentialFrame.TestData.Domain.Aggregates;
using EssentialFrame.TestData.Domain.ValueObjects;

namespace EssentialFrame.TestData.Domain.Rules;

public sealed class TestAggregateTitleCannotBeEmpty : BaseBusinessRule
{
    private readonly TestTitle _title;

    public TestAggregateTitleCannotBeEmpty(Guid aggregateIdentifier, Type aggregateType, TestTitle title) : base(
        aggregateIdentifier, aggregateType)
    {
        _title = title ?? throw new ArgumentNullException(nameof(title));
    }

    public override string Message =>
        $"Unable to set title ({_title}) into aggregate ({_aggregateType.FullName}) with identifier ({_aggregateIdentifier}), because title cannot be empty.";

    public override bool IsBroken()
    {
        return _title.IsEmpty();
    }

    protected override void AddExtraParameters()
    {
        Parameters.TryAdd(nameof(TestAggregateState.Title), _title);
    }
}