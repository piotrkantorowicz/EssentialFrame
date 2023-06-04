using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeImageNameDomainEvent : DomainEventBase
{
    public ChangeImageNameDomainEvent(Guid aggregateIdentifier, IIdentity identity, Guid imageId, string newImageName) :
        base(aggregateIdentifier, identity)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ChangeImageNameDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity, Guid imageId,
        string newImageName) : base(aggregateIdentifier, eventIdentifier, identity)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ChangeImageNameDomainEvent(Guid aggregateIdentifier, IIdentity identity, int expectedVersion, Guid imageId,
        string newImageName) : base(aggregateIdentifier, identity, expectedVersion)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ChangeImageNameDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        int expectedVersion, Guid imageId, string newImageName) : base(aggregateIdentifier, eventIdentifier, identity,
        expectedVersion)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public Guid ImageId { get; }

    public string NewImageName { get; }
}