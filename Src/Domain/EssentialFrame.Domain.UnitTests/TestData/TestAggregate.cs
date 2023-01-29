using System;
using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.UnitTests.TestData;

public sealed class TestAggregate : AggregateRoot
{
    public override AggregateState CreateState()
    {
        return new TestAggregateState();
    }

    public void ChangeTitle(string title)
    {
        ChangeTitleTestDomainEvent @event =
            new ChangeTitleTestDomainEvent(AggregateIdentifier, new TestIdentity(), title);
        Apply(@event);
    }

    public void ChangeDescription(string description)
    {
        ChangeDescriptionTestDomainEvent @event =
            new ChangeDescriptionTestDomainEvent(AggregateIdentifier, new TestIdentity(), description);
        Apply(@event);
    }

    public void ChangeExpirationDate(DateTimeOffset newExpirationDate)
    {
        ExtendExpirationDateTestDomainEvent @event =
            new ExtendExpirationDateTestDomainEvent(AggregateIdentifier, new TestIdentity(), newExpirationDate);

        Apply(@event);
    }
}