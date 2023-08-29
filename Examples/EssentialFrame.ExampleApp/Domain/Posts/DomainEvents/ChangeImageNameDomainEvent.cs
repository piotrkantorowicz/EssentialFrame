using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeImageNameDomainEvent : DomainEvent
{
    public ChangeImageNameDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext, Guid imageId,
        Name newImageName) : base(aggregateIdentifier, identityContext)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ChangeImageNameDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentityContext identityContext,
        Guid imageId, Name newImageName) : base(aggregateIdentifier, eventIdentifier, identityContext)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ChangeImageNameDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext, int expectedVersion,
        Guid imageId, Name newImageName) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ChangeImageNameDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentityContext identityContext,
        int expectedVersion, Guid imageId, Name newImageName) : base(aggregateIdentifier, eventIdentifier,
        identityContext, expectedVersion)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public Guid ImageId { get; }

    public Name NewImageName { get; }
}