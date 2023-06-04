using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeDescriptionDomainEvent : DomainEventBase
{
    public ChangeDescriptionDomainEvent(Guid aggregateIdentifier, IIdentity identity, string newDescription) : base(
        aggregateIdentifier, identity)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        string newDescription) : base(aggregateIdentifier, eventIdentifier, identity)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionDomainEvent(Guid aggregateIdentifier, IIdentity identity, int expectedVersion,
        string newDescription) : base(aggregateIdentifier, identity, expectedVersion)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        int expectedVersion, string newDescription) : base(aggregateIdentifier, eventIdentifier, identity,
        expectedVersion)
    {
        NewDescription = newDescription;
    }

    public string NewDescription { get; }
}