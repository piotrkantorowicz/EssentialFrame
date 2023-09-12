using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class CreateNewPostDomainEvent : DomainEvent<PostIdentifier>
{
    public CreateNewPostDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext, Title title,
        Description description, Date expiration, HashSet<Image> images) : base(aggregateIdentifier, identityContext)
    {
        Title = title;
        Description = description;
        Expiration = expiration;
        Images = images;
    }

    public CreateNewPostDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, Title title, Description description, Date expiration,
        HashSet<Image> images) : base(aggregateIdentifier, eventIdentifier, identityContext)
    {
        Title = title;
        Description = description;
        Expiration = expiration;
        Images = images;
    }

    public CreateNewPostDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext,
        int expectedVersion, Title title, Description description, Date expiration, HashSet<Image> images) : base(
        aggregateIdentifier, identityContext, expectedVersion)
    {
        Title = title;
        Description = description;
        Expiration = expiration;
        Images = images;
    }

    public CreateNewPostDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext, int expectedVersion, Title title, Description description, Date expiration,
        HashSet<Image> images) : base(aggregateIdentifier, eventIdentifier, identityContext, expectedVersion)
    {
        Title = title;
        Description = description;
        Expiration = expiration;
        Images = images;
    }

    public Title Title { get; }

    public Description Description { get; }

    public Date Expiration { get; }

    public HashSet<Image> Images { get; }
}