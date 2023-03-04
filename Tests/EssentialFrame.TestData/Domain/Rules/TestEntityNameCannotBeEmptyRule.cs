using System;
using EssentialFrame.Domain.Rules;

namespace EssentialFrame.TestData.Domain.Rules;

public sealed class TestEntityNameCannotBeEmptyRule : BaseBusinessRule
{
    public TestEntityNameCannotBeEmptyRule(Guid aggregateIdentifier, Type aggregateType) : base(aggregateIdentifier,
        aggregateType)
    {
    }

    public override string Message { get; }

    public override bool IsBroken()
    {
        throw new NotImplementedException();
    }

    protected override void AddExtraParameters()
    {
        throw new NotImplementedException();
    }
}