using System;
using System.Collections.Generic;
using System.Linq;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.Entities;
using EssentialFrame.ExampleApp.Domain.Posts.Rules;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects;

namespace EssentialFrame.ExampleApp.Domain.Posts.Aggregates;

public sealed class PostState : AggregateState
{
    private readonly Guid _aggregateIdentifier;
    private readonly Type _aggregateType;

    private PostState(Guid aggregateId, Type aggregateType)
    {
        _aggregateIdentifier = aggregateId;
        _aggregateType = aggregateType;

        Images = new HashSet<Image>();
    }

    private PostState(Guid aggregateId, Type aggregateType, Title title, string description, DateTimeOffset expiration,
        HashSet<Image> images) : this(aggregateId, aggregateType)
    {
        Title = title;
        Description = description;
        Expiration = expiration;
        Images = images;
    }

    public Title Title { get; private set; }

    public string Description { get; private set; }

    public DateTimeOffset Expiration { get; private set; }

    public HashSet<Image> Images { get; }

    internal static PostState Create(Guid postId, Type type, Title title, string description, DateTimeOffset expiration,
        HashSet<Image> images)
    {
        PostState state = new(postId, type, title, description, expiration, images);

        state.CheckRule(new CannotCreateOutdatedPostRule(postId, type, expiration));

        return state;
    }

    internal static PostState Create(Guid postId, Type type)
    {
        PostState state = new(postId, type);

        return state;
    }

    public void When(ChangeTitleDomainEvent @event)
    {
        CheckRule(new ExpiredPostCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));

        Title = @event.NewTitle;
    }

    public void When(ChangeDescriptionDomainEvent @event)
    {
        CheckRule(new ExpiredPostCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));

        Description = @event.NewDescription;
    }

    public void When(ChangeExpirationDateDomainEvent @event)
    {
        CheckRule(new ExpiredPostCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, @event.NewExpirationDate));

        Expiration = @event.NewExpirationDate;
    }

    public void When(AddImagesDomainEvent @event)
    {
        CheckRule(new ExpiredPostCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));

        foreach (Image newImage in @event.NewImages)
        {
            CheckRule(new ImageNameMustBeUniqueWithInPostRule(_aggregateIdentifier, _aggregateType, newImage.Name,
                Images.Select(i => i.Name).ToArray()));
        }

        HashSet<Image> images = @event.NewImages;

        foreach (Image image in images)
        {
            Images.Add(image);
        }
    }

    public void When(ChangeImageNameDomainEvent @event)
    {
        CheckRule(new ExpiredPostCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));

        CheckRule(new ImageNameMustBeUniqueWithInPostRule(_aggregateIdentifier, _aggregateType, @event.NewImageName,
            Images.Select(i => i.Name).ToArray()));

        Image image = Images.FirstOrDefault(i => i.EntityIdentifier == @event.ImageId);

        image?.UpdateName(@event.NewImageName);
    }
}