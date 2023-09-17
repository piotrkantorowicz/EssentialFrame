using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class DeleteImagesDomainEvent : DomainEvent<PostIdentifier>
{
    public DeleteImagesDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext,
        HashSet<Guid> imagesIds) : base(aggregateIdentifier, identityContext)
    {
        ImagesIds = imagesIds;
    }

    public DeleteImagesDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, HashSet<Guid> imagesIds) : base(aggregateIdentifier, eventIdentifier,
        identityContext)
    {
        ImagesIds = imagesIds;
    }

    public DeleteImagesDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext,
        int expectedVersion, HashSet<Guid> imagesIds) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        ImagesIds = imagesIds;
    }

    public DeleteImagesDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, int expectedVersion, HashSet<Guid> imagesIds) : base(aggregateIdentifier,
        eventIdentifier, identityContext, expectedVersion)
    {
        ImagesIds = imagesIds;
    }

    public HashSet<Guid> ImagesIds { get; }
}