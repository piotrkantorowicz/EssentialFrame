using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;

namespace EssentialFrame.TestData.Domain.DomainEvents;

public class ChangeExpirationDateTestDomainEvent : DomainEventBase
{
    public ChangeExpirationDateTestDomainEvent(Guid aggregateIdentifier, IIdentity identity,
        DateTimeOffset newExpirationDate) : base(aggregateIdentifier, identity)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateTestDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        DateTimeOffset newExpirationDate) : base(aggregateIdentifier, eventIdentifier, identity)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateTestDomainEvent(Guid aggregateIdentifier, IIdentity identity, int expectedVersion,
        DateTimeOffset newExpirationDate) : base(aggregateIdentifier, identity, expectedVersion)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateTestDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        int expectedVersion, DateTimeOffset newExpirationDate) : base(aggregateIdentifier, eventIdentifier, identity,
        expectedVersion)
    {
        NewExpirationDate = newExpirationDate;
    }

    public DateTimeOffset NewExpirationDate { get; }
}