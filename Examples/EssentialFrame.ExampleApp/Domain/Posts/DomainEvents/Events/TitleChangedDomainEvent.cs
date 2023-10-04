using System;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;

namespace EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;

public class TitleChangedDomainEvent : DomainEvent<PostIdentifier, Guid>
{
    public TitleChangedDomainEvent(PostIdentifier aggregateIdentifier, DomainIdentity identityContext,
        Title newTitle) : base(
        aggregateIdentifier, identityContext)
    {
        NewTitle = newTitle;
    }

    public TitleChangedDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        DomainIdentity identityContext,
        Title newTitle) : base(aggregateIdentifier, eventIdentifier, identityContext)
    {
        NewTitle = newTitle;
    }

    public TitleChangedDomainEvent(PostIdentifier aggregateIdentifier, DomainIdentity identityContext,
        int expectedVersion,
        Title newTitle) : base(aggregateIdentifier, identityContext, expectedVersion)
    {
        NewTitle = newTitle;
    }

    public TitleChangedDomainEvent(PostIdentifier aggregateIdentifier, Guid eventIdentifier,
        DomainIdentity identityContext,
        int expectedVersion, Title newTitle) : base(aggregateIdentifier, eventIdentifier, identityContext,
        expectedVersion)
    {
        NewTitle = newTitle;
    }

    public Title NewTitle { get; }
}