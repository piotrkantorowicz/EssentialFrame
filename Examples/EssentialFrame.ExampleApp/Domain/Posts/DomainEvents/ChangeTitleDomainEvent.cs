using System;
using EssentialFrame.Domain.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Identity;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;

public class ChangeTitleDomainEvent : DomainEvent<PostIdentifier>
{
    public ChangeTitleDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext,
        Title newTitle) : base(
        aggregateIdentifier, identityContext)
    {
        NewTitle = newTitle;
    }

    public ChangeTitleDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext,
        Title newTitle) : base(aggregateIdentifier, eventIdentifier, identityContext)
    {
        NewTitle = newTitle;
    }

    public ChangeTitleDomainEvent(PostIdentifier aggregateIdentifier, IIdentityContext identityContext,
        int expectedVersion,
        Title newTitle) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        NewTitle = newTitle;
    }

    public ChangeTitleDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        IIdentityContext identityContext,
        int expectedVersion, Title newTitle) : base(aggregateIdentifier, eventIdentifier, identityContext,
        expectedVersion)
    {
        NewTitle = newTitle;
    }

    public Title NewTitle { get; }
}