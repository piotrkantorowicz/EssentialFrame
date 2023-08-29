using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeExpirationDateDomainEvent : DomainEvent
{
    public ChangeExpirationDateDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext,
        Date newExpirationDate) : base(aggregateIdentifier, identityContext)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, Date newExpirationDate) : base(aggregateIdentifier, eventIdentifier,
        identityContext)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext,
        int expectedVersion, Date newExpirationDate) : base(aggregateIdentifier, identityContext,
        expectedVersion)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, int expectedVersion, Date newExpirationDate) : base(
        aggregateIdentifier, eventIdentifier, identityContext, expectedVersion)
    {
        NewExpirationDate = newExpirationDate;
    }

    public Date NewExpirationDate { get; }
}