using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeImageNameDomainEvent : DomainEventBase
{
    public ChangeImageNameDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext, Guid imageId,
        string newImageName) : base(aggregateIdentifier, identityContext)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ChangeImageNameDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentityContext identityContext,
        Guid imageId, string newImageName) : base(aggregateIdentifier, eventIdentifier, identityContext)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ChangeImageNameDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext, int expectedVersion,
        Guid imageId, string newImageName) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ChangeImageNameDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentityContext identityContext,
        int expectedVersion, Guid imageId, string newImageName) : base(aggregateIdentifier, eventIdentifier,
        identityContext, expectedVersion)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public Guid ImageId { get; }

    public string NewImageName { get; }
}