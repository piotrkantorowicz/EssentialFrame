using System;
using System.Collections.Generic;
using System.Linq;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates.Rules;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;

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
    
    public Title Title { get; private set; }

    public Description Description { get; private set; }

    public Date Expiration { get; private set; }

    public HashSet<Image> Images { get; }

    internal static PostState Create(Guid postId, Type type)
    {
        PostState state = new(postId, type);

        return state;
    }

    public void When(CreateNewPostEvent @event)
    {
        CheckRule(new CannotCreateOutdatedPostRule(_aggregateIdentifier, _aggregateType, @event.Expiration));

        Title = @event.Title;
        Description = @event.Description;
        Expiration = @event.Expiration;

        AddImages(@event.Images ?? new HashSet<Image>());
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

        AddImages(@event.NewImages);
    }

    public void When(ChangeImageNameDomainEvent @event)
    {
        CheckRule(new ExpiredPostCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));

        CheckRule(new PostMustHaveOnlyUniqueImagesRule(_aggregateIdentifier, _aggregateType, @event.NewImageName,
            Images.Select(i => i.Name).ToArray()));

        Image image = Images.FirstOrDefault(i => i.ImageIdentifier == @event.ImageId);

        CheckRule(new PostImageMustExistsWhenUpdatingOrDeletingRule(_aggregateIdentifier, _aggregateType, image));
        
        image?.UpdateName(@event.NewImageName);
    }

    public void When(DeleteImagesEvent @event)
    {
        CheckRule(new ExpiredPostCannotBeUpdatedRule(_aggregateIdentifier, _aggregateType, Expiration));

        foreach (Image image in @event.ImagesIds.Select(imageId =>
                     Images.FirstOrDefault(i => i.ImageIdentifier == imageId)))
        {
            CheckRule(new PostImageMustExistsWhenUpdatingOrDeletingRule(_aggregateIdentifier, _aggregateType, image));

            if (image is not null)
            {
                Images.Remove(image);
            }
        }
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