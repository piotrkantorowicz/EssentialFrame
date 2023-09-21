using System;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;

public class ImageNameChangedDomainEvent : DomainEvent<PostIdentifier>
{
    public ImageNameChangedDomainEvent(PostIdentifier aggregateIdentifier, DomainIdentity identityContext,
        Guid imageId,
        Name newImageName) : base(aggregateIdentifier, identityContext)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ImageNameChangedDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        DomainIdentity identityContext,
        Guid imageId, Name newImageName) : base(aggregateIdentifier, eventIdentifier, identityContext)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ImageNameChangedDomainEvent(PostIdentifier aggregateIdentifier, DomainIdentity identityContext,
        int expectedVersion,
        Guid imageId, Name newImageName) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ImageNameChangedDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        DomainIdentity identityContext,
        int expectedVersion, Guid imageId, Name newImageName) : base(aggregateIdentifier, eventIdentifier,
        identityContext, expectedVersion)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public Guid ImageId { get; }

    public Name NewImageName { get; }
}