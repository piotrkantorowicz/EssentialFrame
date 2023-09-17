using System;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeExpirationDateDomainEvent : DomainEvent<PostIdentifier>
{
    public ChangeExpirationDateDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext,
        Date newExpirationDate) : base(aggregateIdentifier, identityContext)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, Date newExpirationDate) : base(aggregateIdentifier, eventIdentifier,
        identityContext)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext,
        int expectedVersion, Date newExpirationDate) : base(aggregateIdentifier, identityContext,
        expectedVersion)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ChangeExpirationDateDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, int expectedVersion, Date newExpirationDate) : base(
        aggregateIdentifier, eventIdentifier, identityContext, expectedVersion)
    {
        NewExpirationDate = newExpirationDate;
    }

    public Date NewExpirationDate { get; }
}