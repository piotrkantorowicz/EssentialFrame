using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;

namespace EssentialFrame.Domain.UnitTests.TestData;

public class ExtendExpirationDateTestDomainEvent : DomainEventBase
{
    public ExtendExpirationDateTestDomainEvent(Guid aggregateIdentifier, IIdentity identity,
        DateTimeOffset newExpirationDate) : base(aggregateIdentifier, identity)
    {
        NewExpirationDate = newExpirationDate;
    }

    public DateTimeOffset NewExpirationDate { get; }
}