using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeDescriptionDomainEvent : DomainEvent<PostIdentifier>
{
    public ChangeDescriptionDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext,
        Description newDescription) : base(aggregateIdentifier, identityContext)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, Description newDescription) : base(aggregateIdentifier, eventIdentifier,
        identityContext)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext,
        int expectedVersion,
        Description newDescription) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        NewDescription = newDescription;
    }

    public ChangeDescriptionDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, int expectedVersion, Description newDescription) : base(aggregateIdentifier,
        eventIdentifier, identityContext, expectedVersion)
    {
        NewDescription = newDescription;
    }

    public Description NewDescription { get; }
}