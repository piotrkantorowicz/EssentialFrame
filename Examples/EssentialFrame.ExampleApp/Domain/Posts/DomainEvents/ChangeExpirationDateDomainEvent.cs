using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeExpirationDateDomainEvent : DomainEventBase
{
    public ChangeExpirationDateDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext,
        DateTimeOffset newExpirationDate) : base(aggregateIdentifier, identityContext)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, DateTimeOffset newExpirationDate) : base(aggregateIdentifier, eventIdentifier,
        identityContext)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext,
        int expectedVersion, DateTimeOffset newExpirationDate) : base(aggregateIdentifier, identityContext,
        expectedVersion)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, int expectedVersion, DateTimeOffset newExpirationDate) : base(
        aggregateIdentifier, eventIdentifier, identityContext, expectedVersion)
    {
        NewExpirationDate = newExpirationDate;
    }

    public DateTimeOffset NewExpirationDate { get; }
}