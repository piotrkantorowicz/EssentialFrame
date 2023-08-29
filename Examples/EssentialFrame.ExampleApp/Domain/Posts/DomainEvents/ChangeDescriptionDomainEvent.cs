using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeDescriptionDomainEvent : DomainEvent
{
    public ChangeDescriptionDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext,
        Description newDescription) : base(aggregateIdentifier, identityContext)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, Description newDescription) : base(aggregateIdentifier, eventIdentifier,
        identityContext)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext, int expectedVersion,
        Description newDescription) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, int expectedVersion, Description newDescription) : base(aggregateIdentifier,
        eventIdentifier, identityContext, expectedVersion)
    {
        NewDescription = newDescription;
    }

    public Description NewDescription { get; }
}