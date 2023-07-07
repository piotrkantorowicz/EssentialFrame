using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeDescriptionDomainEvent : DomainEventBase
{
    public ChangeDescriptionDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext,
        string newDescription) : base(aggregateIdentifier, identityContext)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, string newDescription) : base(aggregateIdentifier, eventIdentifier,
        identityContext)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext, int expectedVersion,
        string newDescription) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, int expectedVersion, string newDescription) : base(aggregateIdentifier,
        eventIdentifier, identityContext, expectedVersion)
    {
        NewDescription = newDescription;
    }

    public string NewDescription { get; }
}