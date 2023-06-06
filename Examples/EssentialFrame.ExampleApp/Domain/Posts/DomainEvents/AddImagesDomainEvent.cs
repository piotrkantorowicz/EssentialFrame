using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.Posts.Entities;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class AddImagesDomainEvent : DomainEventBase
{
    public AddImagesDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext, HashSet<Image> newImages) :
        base(aggregateIdentifier, identityContext)
    {
        NewImages = newImages;
    }

    public AddImagesDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentityContext identityContext,
        HashSet<Image> newImages) : base(aggregateIdentifier, eventIdentifier, identityContext)
    {
        NewImages = newImages;
    }

    public AddImagesDomainEvent(Guid aggregateIdentifier, IIdentityContext identityContext, int expectedVersion,
        HashSet<Image> newImages) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        NewImages = newImages;
    }

    public AddImagesDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentityContext identityContext,
        int expectedVersion, HashSet<Image> newImages) : base(aggregateIdentifier, eventIdentifier, identityContext,
        expectedVersion)
    {
        NewImages = newImages;
    }

    public HashSet<Image> NewImages { get; }
}