using System;
using System.Collections.Generic;
using Bogus;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Identity.Interfaces;
using EssentialFrame.Tests.Utils;
using EssentialFrame.Time;
using FluentAssertions;
using NUnit.Framework;

namespace EssentialFrame.Domain.Tests.UnitTests.Events;

[TestFixture]
public sealed class DomainEventTests
{
    private readonly Faker _faker = new();

    [Test]
    public void CreateChangeTitleDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        AppIdentityContext appIdentityContext = new();
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        DomainEventIdentifier eventIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        Title title = Title.Default(_faker.Lorem.Sentence());

        // Act
        TitleChangedDomainEvent @event = new(aggregateIdentifier, appIdentityContext, title);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewTitle.Should().Be(title);

        // Act
        @event = new TitleChangedDomainEvent(aggregateIdentifier, eventIdentifier, appIdentityContext, title);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewTitle.Should().Be(title);

        // Act
        @event = new TitleChangedDomainEvent(aggregateIdentifier, appIdentityContext, aggregateVersion, title);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewTitle.Should().Be(title);

        // Act
        @event = new TitleChangedDomainEvent(aggregateIdentifier, eventIdentifier, appIdentityContext, aggregateVersion,
            title);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewTitle.Should().Be(title);
    }

    [Test]
    public void CreateChangeDescriptionDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        AppIdentityContext appIdentityContext = new();
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        DomainEventIdentifier eventIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        Description description = Description.Create(_faker.Lorem.Sentences());

        // Act
        DescriptionChangedDomainEvent @event = new(aggregateIdentifier, appIdentityContext, description);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewDescription.Should().Be(description);

        // Act
        @event = new DescriptionChangedDomainEvent(aggregateIdentifier, eventIdentifier, appIdentityContext,
            description);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewDescription.Should().Be(description);

        // Act
        @event = new DescriptionChangedDomainEvent(aggregateIdentifier, appIdentityContext, aggregateVersion,
            description);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewDescription.Should().Be(description);

        // Act
        @event = new DescriptionChangedDomainEvent(aggregateIdentifier, eventIdentifier, appIdentityContext,
            aggregateVersion, description);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewDescription.Should().Be(description);
    }

    [Test]
    public void CreateChangeExpirationDateDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        AppIdentityContext appIdentityContext = new();
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        DomainEventIdentifier eventIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        Date expirationOffset = Date.Create(_faker.Date.FutureOffset());

        // Act
        ExpirationChangedDateDomainEvent @event = new(aggregateIdentifier, appIdentityContext, expirationOffset);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewExpirationDate.Should().Be(expirationOffset);

        // Act
        @event = new ExpirationChangedDateDomainEvent(aggregateIdentifier, eventIdentifier, appIdentityContext,
            expirationOffset);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewExpirationDate.Should().Be(expirationOffset);

        // Act
        @event = new ExpirationChangedDateDomainEvent(aggregateIdentifier, appIdentityContext, aggregateVersion,
            expirationOffset);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewExpirationDate.Should().Be(expirationOffset);

        // Act
        @event = new ExpirationChangedDateDomainEvent(aggregateIdentifier, eventIdentifier, appIdentityContext,
            aggregateVersion, expirationOffset);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewExpirationDate.Should().Be(expirationOffset);
    }

    [Test]
    public void AddImagesDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        AppIdentityContext appIdentityContext = new();
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        DomainEventIdentifier eventIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        
        HashSet<Image> images = new()
        {
            Image.Create(_faker.Random.Guid(), Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                BytesContent.Create(_faker.Random.Bytes(200))),
            Image.Create(_faker.Random.Guid(), Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                BytesContent.Create(_faker.Random.Bytes(500)))
        };

        // Act
        ImagesAddedDomainEvent @event = new(aggregateIdentifier, appIdentityContext, images);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewImages.Should().BeSameAs(images);

        // Act
        @event = new ImagesAddedDomainEvent(aggregateIdentifier, eventIdentifier, appIdentityContext, images);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewImages.Should().BeSameAs(images);

        // Act
        @event = new ImagesAddedDomainEvent(aggregateIdentifier, appIdentityContext, aggregateVersion, images);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewImages.Should().BeSameAs(images);

        // Act
        @event = new ImagesAddedDomainEvent(aggregateIdentifier, eventIdentifier, appIdentityContext, aggregateVersion,
            images);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewImages.Should().BeSameAs(images);
    }

    [Test]
    public void ChangeImageNameDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        AppIdentityContext appIdentityContext = new();
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        DomainEventIdentifier eventIdentifier = _faker.Random.Guid();
        Guid imageId = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        Name imageName = Name.Create(_faker.Address.City());

        // Act
        ImageNameChangedDomainEvent @event = new(aggregateIdentifier, appIdentityContext, imageId, imageName);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.ImageId.Should().Be(imageId);
        @event.NewImageName.Should().Be(imageName);

        // Act
        @event = new ImageNameChangedDomainEvent(aggregateIdentifier, eventIdentifier, appIdentityContext, imageId,
            imageName);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.ImageId.Should().Be(imageId);
        @event.NewImageName.Should().Be(imageName);

        // Act
        @event = new ImageNameChangedDomainEvent(aggregateIdentifier, appIdentityContext, aggregateVersion, imageId,
            imageName);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.ImageId.Should().Be(imageId);
        @event.NewImageName.Should().Be(imageName);

        // Act
        @event = new ImageNameChangedDomainEvent(aggregateIdentifier, eventIdentifier, appIdentityContext,
            aggregateVersion,
            imageId, imageName);

        // Assert
        AssertIdentity(appIdentityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.ImageId.Should().Be(imageId);
        @event.NewImageName.Should().Be(imageName);
    }

    [Test]
    public void AdjustAggregateVersion_OnValidAggregate_ShouldAssignCorrectAggregateProperties()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Number();

        TitleChangedDomainEvent @event = new(aggregateIdentifier, new AppIdentityContext(),
            Title.Default(_faker.Lorem.Sentence()));

        // Act
        @event.AdjustAggregateVersion(aggregateIdentifier, aggregateVersion);

        // Assert
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.EventTime.Should().BeCloseTo(SystemClock.UtcNow, TimeSpan.FromMilliseconds(Defaults.DefaultCloseTo));
    }

    [Test]
    public void AdjustAggregateVersion_OnInvalidAggregate_ShouldThrowDomainEventDoesNotMatchException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Number();

        TitleChangedDomainEvent @event = new(PostIdentifier.New(_faker.Random.Guid()), new AppIdentityContext(),
            Title.Default(_faker.Lorem.Sentence()));

        // Act
        Action action = () => @event.AdjustAggregateVersion(aggregateIdentifier, aggregateVersion);

        // Assert
        action.Should().ThrowExactly<DomainEventDoesNotMatchException>().WithMessage(
            $"The event is not match aggregate ({aggregateIdentifier}), because was assigned to other aggregate ({@event.AggregateIdentifier})");
    }

    private static void AssertIdentity(IIdentityContext provided, IDomainEvent<PostIdentifier, Guid> expected)
    {
        provided.User.Identifier.Should().Be(expected.DomainEventIdentity.UserIdentifier.Value);
        provided.Service.GetFullIdentifier().Should().Be(expected.DomainEventIdentity.ServiceIdentifier.Value);
        provided.Tenant.Identifier.Should().Be(expected.DomainEventIdentity.TenantIdentifier.Value);
        provided.Correlation.Identifier.Should().Be(expected.DomainEventIdentity.CorrelationIdentifier.Value);
    }
}