using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;

namespace EssentialFrame.TestData.Domain.DomainEvents;

public class ChangeDescriptionTestDomainEvent : DomainEventBase
{
    public ChangeDescriptionTestDomainEvent(Guid aggregateIdentifier, IIdentity identity, string newDescription) : base(
        aggregateIdentifier, identity)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionTestDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        string newDescription) : base(aggregateIdentifier, eventIdentifier, identity)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionTestDomainEvent(Guid aggregateIdentifier, IIdentity identity, int expectedVersion,
        string newDescription) : base(aggregateIdentifier, identity, expectedVersion)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionTestDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        int expectedVersion, string newDescription) : base(aggregateIdentifier, eventIdentifier, identity,
        expectedVersion)
    {
        NewDescription = newDescription;
    }

    public string NewDescription { get; }
}