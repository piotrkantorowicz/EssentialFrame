using System;
using System.Collections.Generic;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.ExampleApp.Domain.PostComments.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.ExampleApp.Domain.Posts.Aggregates;

public sealed class Post : EventSourcingAggregateRoot<PostIdentifier, Guid>
{
    private Post(PostIdentifier aggregateIdentifier) : base(aggregateIdentifier)
    {
    }

    private Post(PostIdentifier aggregateIdentifier, int aggregateVersion) : base(aggregateIdentifier, aggregateVersion)
    {
    }

    private Post(PostIdentifier aggregateIdentifier, int aggregateVersion, TenantIdentifier tenantIdentifier) : base(
        aggregateIdentifier, aggregateVersion, tenantIdentifier)
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
        DomainIdentity domainIdentity)
    {
        NewPostCreatedDomainEvent postCreatedDomainEvent = new(AggregateIdentifier, domainIdentity, title, description,
            expirationDate, images);

        Apply(postCreatedDomainEvent);
    }

    public void ChangeTitle(Title title, DomainIdentity domainIdentity)
    {
        TitleChangedDomainEvent @event = new(AggregateIdentifier, domainIdentity, title);
        Apply(@event);
    }

    public void ChangeDescription(Description description, DomainIdentity domainIdentity)
    {
        DescriptionChangedDomainEvent @event = new(AggregateIdentifier, domainIdentity, description);
        Apply(@event);
    }

    public void ExtendExpirationDate(Date newExpirationDate, DomainIdentity domainIdentity)
    {
        ExpirationChangedDateDomainEvent @event = new(AggregateIdentifier, domainIdentity, newExpirationDate);
        Apply(@event);
    }

    public void AddImages(HashSet<Image> images, DomainIdentity domainIdentity)
    {
        ImagesAddedDomainEvent @event = new(AggregateIdentifier, domainIdentity, images);
        Apply(@event);
    }

    public void ChangeImageName(Guid imageId, Name name, DomainIdentity domainIdentity)
    {
        ImageNameChangedDomainEvent @event = new(AggregateIdentifier, domainIdentity, imageId, name);
        Apply(@event);
    }

    public void DeleteImages(HashSet<Guid> imageIds, DomainIdentity domainIdentity)
    {
        ImagesDeletedDomainEvent @event = new(AggregateIdentifier, domainIdentity, imageIds);
        Apply(@event);
    }

    public void Delete(IReadOnlyCollection<PostCommentIdentifier> postCommentIdentifiers, DomainIdentity domainIdentity)
    {
        PostDeletedDomainEvent @event = new(AggregateIdentifier, domainIdentity, postCommentIdentifiers);
        Apply(@event);
    }
}