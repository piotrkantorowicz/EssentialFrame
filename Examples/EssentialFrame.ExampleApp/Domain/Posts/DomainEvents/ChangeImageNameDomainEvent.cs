using System;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeImageNameDomainEvent : DomainEvent<PostIdentifier>
{
    public ChangeImageNameDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext,
        Guid imageId,
        Name newImageName) : base(aggregateIdentifier, identityContext)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ChangeImageNameDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext,
        Guid imageId, Name newImageName) : base(aggregateIdentifier, eventIdentifier, identityContext)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ChangeImageNameDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext,
        int expectedVersion,
        Guid imageId, Name newImageName) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public ChangeImageNameDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext,
        int expectedVersion, Guid imageId, Name newImageName) : base(aggregateIdentifier, eventIdentifier,
        identityContext, expectedVersion)
    {
        ImageId = imageId;
        NewImageName = newImageName;
    }

    public Guid ImageId { get; }

    public Name NewImageName { get; }
}