using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.Posts.Entities;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class AddImagesDomainEvent : DomainEventBase
{
    public AddImagesDomainEvent(Guid aggregateIdentifier, IIdentity identity, HashSet<Image> newImages) : base(
        aggregateIdentifier, identity)
    {
        NewImages = newImages;
    }

    public AddImagesDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity,
        HashSet<Image> newImages) : base(aggregateIdentifier, eventIdentifier, identity)
    {
        NewImages = newImages;
    }

    public AddImagesDomainEvent(Guid aggregateIdentifier, IIdentity identity, int expectedVersion,
        HashSet<Image> newImages) : base(aggregateIdentifier, identity, expectedVersion)
    {
        NewImages = newImages;
    }

    public AddImagesDomainEvent(Guid aggregateIdentifier, Guid eventIdentifier, IIdentity identity, int expectedVersion,
        HashSet<Image> newImages) : base(aggregateIdentifier, eventIdentifier, identity, expectedVersion)
    {
        NewImages = newImages;
    }

    public HashSet<Image> NewImages { get; }
}