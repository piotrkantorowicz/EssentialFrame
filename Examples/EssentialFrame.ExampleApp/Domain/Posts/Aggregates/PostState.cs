using System;
using System.Collections.Generic;
using System.Linq;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates.Rules;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Time;

namespace EssentialFrame.ExampleApp.Domain.Posts.Aggregates;

public sealed class PostState : EventSourcingAggregateState<PostIdentifier>
{
    private readonly PostIdentifier _aggregateIdentifier;
    private readonly Type _aggregateType;

    private bool _isCreated;

    private PostState(PostIdentifier aggregateId, Type aggregateType)
    {
        _aggregateIdentifier = aggregateId;
        _aggregateType = aggregateType;

        Images = new HashSet<Image>();
    }

    public Guid AuthorIdentifier { get; private set; }

    public Title Title { get; private set; }

    public Description Description { get; private set; }

    public Date Expiration { get; private set; }

    public HashSet<Image> Images { get; }

    public bool IsExpired => Expiration < SystemClock.UtcNow;

    internal static PostState Create(PostIdentifier postIdentifier, Type type)
    {
        PostState state = new(postIdentifier, type);

        return state;
    }

    public void When(NewPostCreatedDomainEvent postCreatedDomainEvent)
    {
        CheckRule(new CannotCreateOutdatedPostRule(_aggregateIdentifier, _aggregateType,
            postCreatedDomainEvent.Expiration));

        Title = postCreatedDomainEvent.Title;
        Description = postCreatedDomainEvent.Description;
        Expiration = postCreatedDomainEvent.Expiration;
        AuthorIdentifier = postCreatedDomainEvent.DomainEventIdentity.UserIdentifier.Value;

        AddImages(postCreatedDomainEvent.Images ?? new HashSet<Image>());

        _isCreated = true;
    }

    public void When(TitleChangedDomainEvent @event)
    {
        CheckRule(new ExpiredPostCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));
        CheckRule(new PostMustBeCreatedBeforeBeModifiedRule(_aggregateIdentifier, _aggregateType, _isCreated));

        CheckRule(new PostCanBeOnlyUpdatedOrDeletedByAuthorRule(_aggregateIdentifier, _aggregateType,
            @event.DomainEventIdentity.UserIdentifier.Value,
            AuthorIdentifier));

        Title = @event.NewTitle;
    }

    public void When(DescriptionChangedDomainEvent @event)
    {
        CheckRule(new ExpiredPostCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));
        CheckRule(new PostMustBeCreatedBeforeBeModifiedRule(_aggregateIdentifier, _aggregateType, _isCreated));

        CheckRule(new PostCanBeOnlyUpdatedOrDeletedByAuthorRule(_aggregateIdentifier, _aggregateType,
            @event.DomainEventIdentity.UserIdentifier.Value,
            AuthorIdentifier));

        Description = @event.NewDescription;
    }

    public void When(ExpirationChangedDateDomainEvent @event)
    {
        CheckRule(new ExpiredPostCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, @event.NewExpirationDate));
        CheckRule(new PostMustBeCreatedBeforeBeModifiedRule(_aggregateIdentifier, _aggregateType, _isCreated));

        CheckRule(new PostCanBeOnlyUpdatedOrDeletedByAuthorRule(_aggregateIdentifier, _aggregateType,
            @event.DomainEventIdentity.UserIdentifier.Value,
            AuthorIdentifier));

        Expiration = @event.NewExpirationDate;
    }

    public void When(ImagesAddedDomainEvent @event)
    {
        CheckRule(new ExpiredPostCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));
        CheckRule(new PostMustBeCreatedBeforeBeModifiedRule(_aggregateIdentifier, _aggregateType, _isCreated));

        CheckRule(new PostCanBeOnlyUpdatedOrDeletedByAuthorRule(_aggregateIdentifier, _aggregateType,
            @event.DomainEventIdentity.UserIdentifier.Value,
            AuthorIdentifier));

        AddImages(@event.NewImages);
    }

    public void When(ImageNameChangedDomainEvent @event)
    {
        CheckRule(new ExpiredPostCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));
        CheckRule(new PostMustBeCreatedBeforeBeModifiedRule(_aggregateIdentifier, _aggregateType, _isCreated));

        CheckRule(new PostCanBeOnlyUpdatedOrDeletedByAuthorRule(_aggregateIdentifier, _aggregateType,
            @event.DomainEventIdentity.UserIdentifier.Value,
            AuthorIdentifier));

        CheckRule(new PostMustHaveOnlyUniqueImagesRule(_aggregateIdentifier, _aggregateType, @event.NewImageName,
            Images.Select(i => i.Name).ToArray()));

        Image image = Images.FirstOrDefault(i => i.EntityIdentifier == @event.ImageId);

        CheckRule(new PostImageMustExistsWhenUpdatingOrDeletingRule(_aggregateIdentifier, _aggregateType, image));

        image?.UpdateName(@event.NewImageName);
    }

    public void When(ImagesDeletedDomainEvent @event)
    {
        CheckRule(new ExpiredPostCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));
        CheckRule(new PostMustBeCreatedBeforeBeModifiedRule(_aggregateIdentifier, _aggregateType, _isCreated));

        CheckRule(new PostCanBeOnlyUpdatedOrDeletedByAuthorRule(_aggregateIdentifier, _aggregateType,
            @event.DomainEventIdentity.UserIdentifier.Value,
            AuthorIdentifier));

        foreach (Image image in @event.ImagesIds.Select(imageId =>
                     Images.FirstOrDefault(i => i.EntityIdentifier == imageId)))
        {
            CheckRule(new PostImageMustExistsWhenUpdatingOrDeletingRule(_aggregateIdentifier, _aggregateType, image));

            if (image is not null)
            {
                Images.Remove(image);
            }
        }
    }

    public void When(PostDeletedDomainEvent @event)
    {
        CheckRule(new PostMustBeCreatedBeforeBeModifiedRule(_aggregateIdentifier, _aggregateType, _isCreated));

        CheckRule(new PostCanBeOnlyUpdatedOrDeletedByAuthorRule(_aggregateIdentifier, _aggregateType,
            @event.DomainEventIdentity.UserIdentifier.Value, AuthorIdentifier));
    }

    private void AddImages(HashSet<Image> images)
    {
        foreach (Image newImage in images)
        {
            CheckRule(new PostMustHaveOnlyUniqueImagesRule(_aggregateIdentifier, _aggregateType, newImage.Name,
                Images.Select(i => i.Name).ToArray()));

            Images.Add(newImage);
        }
    }
}