using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;
using EssentialFrame.TestData.Domain.ValueObjects;

namespace EssentialFrame.TestData.Domain.DomainEvents;

public class ChangeTitleTestDomainEvent : DomainEventBase
{
    public ChangeTitleTestDomainEvent(Guid aggregateIdentifier, IIdentity identity, TestTitle newTitle) : base(
        aggregateIdentifier, identity)
    {
        NewTitle = newTitle;
    }

    public ChangeTitleTestDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        TestTitle newTitle) : base(aggregateIdentifier, eventIdentifier, identity)
    {
        NewTitle = newTitle;
    }

    public ChangeTitleTestDomainEvent(Guid aggregateIdentifier, IIdentity identity, int expectedVersion,
        TestTitle newTitle) : base(aggregateIdentifier, identity, expectedVersion)
    {
        NewTitle = newTitle;
    }

    public ChangeTitleTestDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        int expectedVersion, TestTitle newTitle) : base(aggregateIdentifier, eventIdentifier, identity, expectedVersion)
    {
        NewTitle = newTitle;
    }

    public TestTitle NewTitle { get; }
}