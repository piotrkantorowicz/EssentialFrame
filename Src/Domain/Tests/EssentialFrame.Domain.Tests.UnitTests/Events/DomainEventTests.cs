using System;
using System.Collections.Generic;
using Bogus;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Identity;
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
        IdentityContext identityContext = new();
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        Guid eventIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        Title title = Title.Default(_faker.Lorem.Sentence());

        // Act
        ChangeTitleDomainEvent @event = new(aggregateIdentifier, identityContext, title);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewTitle.Should().Be(title);

        // Act
        @event = new ChangeTitleDomainEvent(aggregateIdentifier, eventIdentifier, identityContext, title);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewTitle.Should().Be(title);

        // Act
        @event = new ChangeTitleDomainEvent(aggregateIdentifier, identityContext, aggregateVersion, title);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewTitle.Should().Be(title);

        // Act
        @event = new ChangeTitleDomainEvent(aggregateIdentifier, eventIdentifier, identityContext, aggregateVersion,
            title);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewTitle.Should().Be(title);
    }

    [Test]
    public void CreateChangeDescriptionDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        IdentityContext identityContext = new();
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        Guid eventIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        Description description = Description.Create(_faker.Lorem.Sentences());

        // Act
        ChangeDescriptionDomainEvent @event = new(aggregateIdentifier, identityContext, description);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewDescription.Should().Be(description);

        // Act
        @event = new ChangeDescriptionDomainEvent(aggregateIdentifier, eventIdentifier, identityContext, description);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewDescription.Should().Be(description);

        // Act
        @event = new ChangeDescriptionDomainEvent(aggregateIdentifier, identityContext, aggregateVersion, description);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewDescription.Should().Be(description);

        // Act
        @event = new ChangeDescriptionDomainEvent(aggregateIdentifier, eventIdentifier, identityContext,
            aggregateVersion, description);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewDescription.Should().Be(description);
    }

    [Test]
    public void CreateChangeExpirationDateDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        IdentityContext identityContext = new();
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        Guid eventIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        Date expirationOffset = Date.Create(_faker.Date.FutureOffset());

        // Act
        ChangeExpirationDateDomainEvent @event = new(aggregateIdentifier, identityContext, expirationOffset);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewExpirationDate.Should().Be(expirationOffset);

        // Act
        @event = new ChangeExpirationDateDomainEvent(aggregateIdentifier, eventIdentifier, identityContext,
            expirationOffset);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewExpirationDate.Should().Be(expirationOffset);

        // Act
        @event = new ChangeExpirationDateDomainEvent(aggregateIdentifier, identityContext, aggregateVersion,
            expirationOffset);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewExpirationDate.Should().Be(expirationOffset);

        // Act
        @event = new ChangeExpirationDateDomainEvent(aggregateIdentifier, eventIdentifier, identityContext,
            aggregateVersion, expirationOffset);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewExpirationDate.Should().Be(expirationOffset);
    }

    [Test]
    public void AddImagesDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        IdentityContext identityContext = new();
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        Guid eventIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        
        HashSet<Image> images = new()
        {
            Image.Create(_faker.Random.Guid(), Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                BytesContent.Create(_faker.Random.Bytes(200))),
            Image.Create(_faker.Random.Guid(), Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                BytesContent.Create(_faker.Random.Bytes(500)))
        };

        // Act
        AddImagesDomainEvent @event = new(aggregateIdentifier, identityContext, images);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewImages.Should().BeSameAs(images);

        // Act
        @event = new AddImagesDomainEvent(aggregateIdentifier, eventIdentifier, identityContext, images);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.NewImages.Should().BeSameAs(images);

        // Act
        @event = new AddImagesDomainEvent(aggregateIdentifier, identityContext, aggregateVersion, images);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewImages.Should().BeSameAs(images);

        // Act
        @event = new AddImagesDomainEvent(aggregateIdentifier, eventIdentifier, identityContext, aggregateVersion,
            images);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.NewImages.Should().BeSameAs(images);
    }

    [Test]
    public void ChangeImageNameDomainEvent_Always_ShouldAssignCorrectValues()
    {
        // Arrange
        IdentityContext identityContext = new();
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        Guid eventIdentifier = _faker.Random.Guid();
        Guid imageId = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        Name imageName = Name.Create(_faker.Address.City());

        // Act
        ChangeImageNameDomainEvent @event = new(aggregateIdentifier, identityContext, imageId, imageName);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.ImageId.Should().Be(imageId);
        @event.NewImageName.Should().Be(imageName);

        // Act
        @event = new ChangeImageNameDomainEvent(aggregateIdentifier, eventIdentifier, identityContext, imageId,
            imageName);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().Be(eventIdentifier);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(0);
        @event.ImageId.Should().Be(imageId);
        @event.NewImageName.Should().Be(imageName);

        // Act
        @event = new ChangeImageNameDomainEvent(aggregateIdentifier, identityContext, aggregateVersion, imageId,
            imageName);

        // Assert
        AssertIdentity(identityContext, @event);
        @event.EventIdentifier.Should().NotBe(Guid.Empty);
        @event.AggregateIdentifier.Should().Be(aggregateIdentifier);
        @event.AggregateVersion.Should().Be(aggregateVersion);
        @event.ImageId.Should().Be(imageId);
        @event.NewImageName.Should().Be(imageName);

        // Act
        @event = new ChangeImageNameDomainEvent(aggregateIdentifier, eventIdentifier, identityContext, aggregateVersion,
            imageId, imageName);

        // Assert
        AssertIdentity(identityContext, @event);
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

        ChangeTitleDomainEvent @event = new(aggregateIdentifier, new IdentityContext(),
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

        ChangeTitleDomainEvent @event = new(PostIdentifier.New(_faker.Random.Guid()), new IdentityContext(),
            Title.Default(_faker.Lorem.Sentence()));

        // Act
        Action action = () => @event.AdjustAggregateVersion(aggregateIdentifier, aggregateVersion);

        // Assert
        action.Should().ThrowExactly<DomainEventDoesNotMatchException>().WithMessage(
            $"The event is not match aggregate ({aggregateIdentifier}), because was assigned to other aggregate ({@event.AggregateIdentifier})");
    }

    private static void AssertIdentity(IIdentityContext provided, IDomainEvent<PostIdentifier> expected)
    {
        provided.User.Identifier.Should().Be(expected.DomainEventIdentity.UserIdentity.Value);
        provided.Service.GetFullIdentifier().Should().Be(expected.DomainEventIdentity.ServiceIdentity.Value);
        provided.Tenant.Identifier.Should().Be(expected.DomainEventIdentity.TenantIdentity.Value);
        provided.Correlation.Identifier.Should().Be(expected.DomainEventIdentity.CorrelationIdentity.Value);
    }
}