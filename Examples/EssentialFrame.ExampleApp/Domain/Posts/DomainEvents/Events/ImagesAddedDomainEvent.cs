using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;

public class ImagesAddedDomainEvent : DomainEvent<PostIdentifier>
{
    public ImagesAddedDomainEvent(PostIdentifier aggregateIdentifier, DomainIdentity domainIdentity,
        HashSet<Image> newImages) : base(aggregateIdentifier, domainIdentity)
    {
        NewImages = newImages;
    }

    public ImagesAddedDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        DomainIdentity identityContext,
        HashSet<Image> newImages) : base(aggregateIdentifier, eventIdentifier, identityContext)
    {
        NewImages = newImages;
    }

    public ImagesAddedDomainEvent(PostIdentifier aggregateIdentifier, DomainIdentity domainIdentity,
        int expectedVersion, HashSet<Image> newImages) : base(aggregateIdentifier, domainIdentity, expectedVersion)
    {
        NewImages = newImages;
    }

    public ImagesAddedDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        DomainIdentity identityContext,
        int expectedVersion, HashSet<Image> newImages) : base(aggregateIdentifier, eventIdentifier, identityContext,
        expectedVersion)
    {
        NewImages = newImages;
    }

    public HashSet<Image> NewImages { get; }
}