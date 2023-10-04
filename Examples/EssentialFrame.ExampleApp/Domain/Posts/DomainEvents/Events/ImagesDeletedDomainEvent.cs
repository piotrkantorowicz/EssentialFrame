using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;

public class ImagesDeletedDomainEvent : DomainEvent<PostIdentifier, Guid>
{
    public ImagesDeletedDomainEvent(PostIdentifier aggregateIdentifier, DomainIdentity identityContext,
        HashSet<Guid> imagesIds) : base(aggregateIdentifier, identityContext)
    {
        ImagesIds = imagesIds;
    }

    public ImagesDeletedDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        DomainIdentity identityContext, HashSet<Guid> imagesIds) : base(aggregateIdentifier, eventIdentifier,
        identityContext)
    {
        ImagesIds = imagesIds;
    }

    public ImagesDeletedDomainEvent(PostIdentifier aggregateIdentifier, DomainIdentity identityContext,
        int expectedVersion, HashSet<Guid> imagesIds) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        ImagesIds = imagesIds;
    }

    public ImagesDeletedDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        DomainIdentity identityContext, int expectedVersion, HashSet<Guid> imagesIds) : base(aggregateIdentifier,
        eventIdentifier, identityContext, expectedVersion)
    {
        ImagesIds = imagesIds;
    }

    public HashSet<Guid> ImagesIds { get; }
}