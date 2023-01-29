using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;

namespace EssentialFrame.Domain.UnitTests.TestData;

public class ChangeTitleTestDomainEvent : DomainEventBase
{
    public ChangeTitleTestDomainEvent(Guid aggregateIdentifier, IIdentity identity, string newTitle) : base(
        aggregateIdentifier, identity)
    {
        NewTitle = newTitle;
    }

    public string NewTitle { get; }
}