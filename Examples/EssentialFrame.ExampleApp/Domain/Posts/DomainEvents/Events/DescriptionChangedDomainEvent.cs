using System;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;

public class DescriptionChangedDomainEvent : DomainEvent<PostIdentifier, Guid>
{
    public DescriptionChangedDomainEvent(PostIdentifier aggregateIdentifier, DomainIdentity identityContext,
        Description newDescription) : base(aggregateIdentifier, identityContext)
    {
        NewDescription = newDescription;
    }

    public DescriptionChangedDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        DomainIdentity domainIdentity, Description newDescription) : base(aggregateIdentifier, eventIdentifier,
        domainIdentity)
    {
        NewDescription = newDescription;
    }

    public DescriptionChangedDomainEvent(PostIdentifier aggregateIdentifier, DomainIdentity identityContext,
        int expectedVersion,
        Description newDescription) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        NewDescription = newDescription;
    }

    public DescriptionChangedDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        DomainIdentity domainIdentity, int expectedVersion, Description newDescription) : base(aggregateIdentifier,
        eventIdentifier, domainIdentity, expectedVersion)
    {
        NewDescription = newDescription;
    }

    public Description NewDescription { get; }
}