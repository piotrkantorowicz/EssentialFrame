using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.ExampleApp.Domain.Posts.Aggregates;

public sealed class Post : AggregateRoot
{
    private Post(IIdentityContext identityContext) : base(identityContext)
    {
    }

    private Post(Guid aggregateIdentifier, IIdentityContext identityContext) : base(aggregateIdentifier,
        identityContext)
    {
    }

    private Post(int aggregateVersion, IIdentityContext identityContext) : base(aggregateVersion, identityContext)
    {
    }

    private Post(Guid aggregateIdentifier, int aggregateVersion, IIdentityContext identityContext) : base(
        aggregateIdentifier, aggregateVersion, identityContext)
    {
    }

    public override PostState CreateState()
    {
        return PostState.Create(AggregateIdentifier, GetType());
    }

    public override void RestoreState(object aggregateState, ISerializer serializer = null)
    {
        switch (aggregateState)
        {
            case null:
                return;
            case string serializedState:
                State = serializer?.Deserialize<PostState>(serializedState, typeof(PostState));
                return;
            default:
                State = (PostState)aggregateState;
                break;
        }
    }

    public void Create(Title title, Description description, Date expirationDate, HashSet<Image> images)
    {
        CreateNewPostDomainEvent domainEvent = new(AggregateIdentifier, IdentityContext, title, description,
            expirationDate,
            images);

        Apply(domainEvent);
    }

    public void ChangeTitle(Title title)
    {
        ChangeTitleDomainEvent @event = new(AggregateIdentifier, IdentityContext, title);
        Apply(@event);
    }

    public void ChangeDescription(Description description)
    {
        ChangeDescriptionDomainEvent @event = new(AggregateIdentifier, IdentityContext, description);
        Apply(@event);
    }

    public void ExtendExpirationDate(Date newExpirationDate)
    {
        ChangeExpirationDateDomainEvent @event = new(AggregateIdentifier, IdentityContext, newExpirationDate);
        Apply(@event);
    }

    public void AddImages(HashSet<Image> images)
    {
        AddImagesDomainEvent @event = new(AggregateIdentifier, IdentityContext, images);
        Apply(@event);
    }

    public void ChangeImageName(Guid imageId, Name name)
    {
        ChangeImageNameDomainEvent @event = new(AggregateIdentifier, IdentityContext, imageId, name);
        Apply(@event);
    }
}