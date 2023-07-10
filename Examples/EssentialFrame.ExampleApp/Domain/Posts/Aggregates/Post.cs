using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.Entities;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.ExampleApp.Domain.Posts.Aggregates;

public sealed class Post : AggregateRoot
{
    private Post(Guid aggregateIdentifier, int aggregateVersion) : base(aggregateIdentifier, aggregateVersion)
    {
    }

    public override PostState CreateState()
    {
        return PostState.Create(AggregateIdentifier, GetType());
    }

    public PostState CreateState(Title title, string description, DateTimeOffset expiration, HashSet<Image> images)
    {
        return PostState.Create(AggregateIdentifier, GetType(), title, description, expiration, images);
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

    public void ChangeTitle(Title title, IIdentityContext identityContext = null)
    {
        ChangeTitleDomainEvent @event = new(AggregateIdentifier, identityContext, title);
        Apply(@event);
    }

    public void ChangeDescription(string description, IIdentityContext identityContext = null)
    {
        ChangeDescriptionDomainEvent @event = new(AggregateIdentifier, identityContext, description);
        Apply(@event);
    }

    public void ExtendExpirationDate(DateTimeOffset newExpirationDate, IIdentityContext identityContext = null)
    {
        ChangeExpirationDateDomainEvent @event = new(AggregateIdentifier, identityContext, newExpirationDate);
        Apply(@event);
    }

    public void AddImages(HashSet<Image> images, IIdentityContext identityContext = null)
    {
        AddImagesDomainEvent @event = new(AggregateIdentifier, identityContext, images);
        Apply(@event);
    }

    public void ChangeImageName(Guid imageId, string name, IIdentityContext identityContext = null)
    {
        ChangeImageNameDomainEvent @event = new(AggregateIdentifier, identityContext, imageId, name);
        Apply(@event);
    }
}