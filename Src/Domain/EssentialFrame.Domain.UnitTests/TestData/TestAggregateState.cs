using System;
using EssentialFrame.Domain.Aggregates;

namespace EssentialFrame.Domain.UnitTests.TestData;

public class TestAggregateState : AggregateState
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTimeOffset Expiration { get; set; }

    public void When(ChangeTitleTestDomainEvent @event)
    {
        Title = @event.NewTitle;
    }

    public void When(ChangeDescriptionTestDomainEvent @event)
    {
        Description = @event.NewDescription;
    }

    public void When(ExtendExpirationDateTestDomainEvent @event)
    {
        Expiration = @event.NewExpirationDate;
    }
}