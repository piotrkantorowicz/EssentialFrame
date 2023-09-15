using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class AddImagesDomainEvent : DomainEvent<PostIdentifier>
{
    public AddImagesDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext,
        HashSet<Image> newImages) :
        base(aggregateIdentifier, identityContext)
    {
        NewImages = newImages;
    }

    public AddImagesDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext,
        HashSet<Image> newImages) : base(aggregateIdentifier, eventIdentifier, identityContext)
    {
        NewImages = newImages;
    }

    public AddImagesDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext,
        int expectedVersion,
        HashSet<Image> newImages) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        NewImages = newImages;
    }

    public AddImagesDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext,
        int expectedVersion, HashSet<Image> newImages) : base(aggregateIdentifier, eventIdentifier, identityContext,
        expectedVersion)
    {
        NewImages = newImages;
    }

    public HashSet<Image> NewImages { get; }
}