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

    private Post(Guid aggregateIdentifier, int aggregateVersion, IIdentityService identityService) : base(
        aggregateIdentifier, aggregateVersion, identityService)
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

    public void ChangeTitle(Title title)
    {
        ChangeTitleDomainEvent @event = new(AggregateIdentifier, GetIdentity(), title);
        Apply(@event);
    }

    public void ChangeDescription(string description)
    {
        ChangeDescriptionDomainEvent @event = new(AggregateIdentifier, GetIdentity(), description);
        Apply(@event);
    }

    public void ExtendExpirationDate(DateTimeOffset newExpirationDate)
    {
        ChangeExpirationDateDomainEvent @event = new(AggregateIdentifier, GetIdentity(), newExpirationDate);
        Apply(@event);
    }

    public void AddImages(HashSet<Image> images)
    {
        AddImagesDomainEvent @event = new(AggregateIdentifier, GetIdentity(), images);
        Apply(@event);
    }

    public void ChangeImageName(Guid imageId, string name)
    {
        ChangeImageNameDomainEvent @event = new(AggregateIdentifier, GetIdentity(), imageId, name);
        Apply(@event);
    }
}