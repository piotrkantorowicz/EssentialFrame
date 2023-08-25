using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class DeleteImagesEvent : DomainEvent
{
    public DeleteImagesEvent(Guid aggregateIdentifier, IIdentityContext identityContext, HashSet<Guid> imagesIds) :
        base(aggregateIdentifier, identityContext)
    {
        ImagesIds = imagesIds;
    }

    public DeleteImagesEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentityContext identityContext,
        HashSet<Guid> imagesIds) : base(aggregateIdentifier, eventIdentifier, identityContext)
    {
        ImagesIds = imagesIds;
    }

    public DeleteImagesEvent(Guid aggregateIdentifier, IIdentityContext identityContext, int expectedVersion,
        HashSet<Guid> imagesIds) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        ImagesIds = imagesIds;
    }

    public DeleteImagesEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentityContext identityContext,
        int expectedVersion, HashSet<Guid> imagesIds) : base(aggregateIdentifier, eventIdentifier, identityContext,
        expectedVersion)
    {
        ImagesIds = imagesIds;
    }

    public HashSet<Guid> ImagesIds { get; }
}