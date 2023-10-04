using System;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;

public class ExpirationChangedDateDomainEvent : DomainEvent<PostIdentifier, Guid>
{
    public ExpirationChangedDateDomainEvent(PostIdentifier aggregateIdentifier, DomainIdentity identityContext,
        Date newExpirationDate) : base(aggregateIdentifier, identityContext)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ExpirationChangedDateDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        DomainIdentity identityContext, Date newExpirationDate) : base(aggregateIdentifier, eventIdentifier,
        identityContext)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ExpirationChangedDateDomainEvent(PostIdentifier aggregateIdentifier, DomainIdentity identityContext,
        int expectedVersion, Date newExpirationDate) : base(aggregateIdentifier, identityContext,
        expectedVersion)
    {
        NewExpirationDate = newExpirationDate;
    }

    public ExpirationChangedDateDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        DomainIdentity identityContext, int expectedVersion, Date newExpirationDate) : base(
        aggregateIdentifier, eventIdentifier, identityContext, expectedVersion)
    {
        NewExpirationDate = newExpirationDate;
    }

    public Date NewExpirationDate { get; }
}