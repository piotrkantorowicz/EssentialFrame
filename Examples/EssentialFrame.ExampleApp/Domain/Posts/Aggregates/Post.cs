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
    private Post(Guid aggregateIdentifier) : base(aggregateIdentifier)
    {
    }

    private Post(Guid aggregateIdentifier, int aggregateVersion) : base(aggregateIdentifier, aggregateVersion)
    {
    }

    private Post(Guid aggregateIdentifier, int aggregateVersion, Guid tenantIdentifier) : base(aggregateIdentifier,
        aggregateVersion, tenantIdentifier)
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

    public void Create(Title title, Description description, Date expirationDate, HashSet<Image> images,
        IIdentityContext identityContext)
    {
        CreateNewPostDomainEvent domainEvent = new(AggregateIdentifier, identityContext, title, description,
            expirationDate, images);

        Apply(domainEvent);
    }

    public void ChangeTitle(Title title, IIdentityContext identityContext)
    {
        ChangeTitleDomainEvent @event = new(AggregateIdentifier, identityContext, title);
        Apply(@event);
    }

    public void ChangeDescription(Description description, IIdentityContext identityContext)
    {
        ChangeDescriptionDomainEvent @event = new(AggregateIdentifier, identityContext, description);
        Apply(@event);
    }

    public void ExtendExpirationDate(Date newExpirationDate, IIdentityContext identityContext)
    {
        ChangeExpirationDateDomainEvent @event = new(AggregateIdentifier, identityContext, newExpirationDate);
        Apply(@event);
    }

    public void AddImages(HashSet<Image> images, IIdentityContext identityContext)
    {
        AddImagesDomainEvent @event = new(AggregateIdentifier, identityContext, images);
        Apply(@event);
    }

    public void ChangeImageName(Guid imageId, Name name, IIdentityContext identityContext)
    {
        ChangeImageNameDomainEvent @event = new(AggregateIdentifier, identityContext, imageId, name);
        Apply(@event);
    }

    public void DeleteImages(HashSet<Guid> imageIds, IIdentityContext identityContext)
    {
        DeleteImagesDomainEvent @event = new(AggregateIdentifier, identityContext, imageIds);
        Apply(@event);
    }
}