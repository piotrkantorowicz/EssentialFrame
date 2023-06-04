using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeExpirationDateDomainEvent : DomainEventBase
{
    public ChangeExpirationDateDomainEvent(Guid aggregateIdentifier, IIdentity identity,
        DateTimeOffset newExpirationDate) : base(aggregateIdentifier, identity)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        DateTimeOffset newExpirationDate) : base(aggregateIdentifier, eventIdentifier, identity)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateDomainEvent(Guid aggregateIdentifier, IIdentity identity, int expectedVersion,
        DateTimeOffset newExpirationDate) : base(aggregateIdentifier, identity, expectedVersion)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        int expectedVersion, DateTimeOffset newExpirationDate) : base(aggregateIdentifier, eventIdentifier, identity,
        expectedVersion)
    {
        NewExpirationDate = newExpirationDate;
    }

    public DateTimeOffset NewExpirationDate { get; }
}