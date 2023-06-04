using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeTitleDomainEvent : DomainEventBase
{
    public ChangeTitleDomainEvent(Guid aggregateIdentifier, IIdentity identity, Title newTitle) : base(
        aggregateIdentifier, identity)
    {
        NewTitle = newTitle;
    }

    public ChangeTitleDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity, Title newTitle) :
        base(aggregateIdentifier, eventIdentifier, identity)
    {
        NewTitle = newTitle;
    }

    public ChangeTitleDomainEvent(Guid aggregateIdentifier, IIdentity identity, int expectedVersion, Title newTitle) :
        base(aggregateIdentifier, identity, expectedVersion)
    {
        NewTitle = newTitle;
    }

    public ChangeTitleDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        int expectedVersion, Title newTitle) : base(aggregateIdentifier, eventIdentifier, identity, expectedVersion)
    {
        NewTitle = newTitle;
    }

    public Title NewTitle { get; }
}