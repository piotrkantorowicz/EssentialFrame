using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Domain.Events;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Domain.Factories;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.Entities;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects;
using EssentialFrame.Extensions;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.Tests.Utils;
using EssentialFrame.Time;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Tests.UnitTests.Aggregates;

[TestFixture]
public sealed class AggregateRootTests
{
    private readonly Faker _faker = new();
    private readonly Mock<ISerializer> _serializerMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();

    [Test]
    public void CreateState_Always_ShouldReturnSpecificType()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new ExampleApp.Domain.Posts.Identity.Identity());

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        // Act
        PostState state = aggregate.CreateState();

        // Assert
        state.Should().BeOfType(typeof(PostState));
    }

    [Test]
    public void RestoreState_Always_ShouldAssignCorrectState()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();
        string serializedState = _faker.Lorem.Sentence();

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new ExampleApp.Domain.Posts.Identity.Identity());

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        PostState expectedAggregateState = aggregate.CreateState(Title.Create(_faker.Lorem.Sentence(), false),
            _faker.Lorem.Sentences(), _faker.Date.FutureOffset(), new HashSet<Image>
            {
                Image.Create(_faker.Random.Guid(), _faker.Lorem.Word(), _faker.Random.Bytes(300))
            });

        aggregate.RestoreState(expectedAggregateState);

        _serializerMock.Setup(s => s.Deserialize<PostState>(serializedState, typeof(PostState)))
            .Returns(expectedAggregateState);

        // Act
        aggregate.RestoreState(serializedState, _serializerMock.Object);

        // Assert
        aggregate.State.Should().BeOfType(typeof(PostState));

        PostState aggregateState = (PostState)aggregate.State;
        AssertState(aggregateState, expectedAggregateState);

        // Act
        aggregate.RestoreState(expectedAggregateState);

        // Assert
        AssertState(aggregateState, expectedAggregateState);
    }

    [Test]
    public void ApplyEvent_Always_ShouldModifyAggregateState()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new ExampleApp.Domain.Posts.Identity.Identity());

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        Title expectedTitle = Title.Create(_faker.Lorem.Sentence(), true);
        string expectedDescription = _faker.Lorem.Sentences();
        DateTimeOffset expectedExpiration = _faker.Date.FutureOffset();

        // Act
        aggregate.ChangeTitle(expectedTitle);

        // Assert
        PostState aggregateState = (PostState)aggregate.State;
        aggregateState.Title.Should().Be(expectedTitle);

        // Act
        aggregate.ChangeDescription(expectedDescription);

        // Assert
        aggregateState = (PostState)aggregate.State;
        aggregateState.Description.Should().Be(expectedDescription);

        // Act
        aggregate.ExtendExpirationDate(expectedExpiration);

        // Assert
        aggregateState = (PostState)aggregate.State;
        aggregateState.Expiration.Should().Be(expectedExpiration);
    }

    [Test]
    public void GetUncommittedChanges_Always_ShouldGetAllChangesFromAggregate()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new ExampleApp.Domain.Posts.Identity.Identity());

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        Title expectedTitle = Title.Create(_faker.Lorem.Sentence(), true);
        string expectedDescription = _faker.Lorem.Sentences();
        DateTimeOffset expectedExpiration = _faker.Date.FutureOffset();

        // Act
        aggregate.ChangeTitle(expectedTitle);
        aggregate.ChangeDescription(expectedDescription);
        aggregate.ExtendExpirationDate(expectedExpiration);

        // Assert
        IDomainEvent[] changes = aggregate.GetUncommittedChanges();
        changes.Should().HaveCount(3);
    }

    [Test]
    public void FlushUncommittedChanges_Always_ShouldIncreaseAggregateVersion()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new ExampleApp.Domain.Posts.Identity.Identity());

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        Title expectedTitle = Title.Create(_faker.Lorem.Sentence(), true);
        string expectedDescription = _faker.Lorem.Sentences();
        DateTimeOffset expectedExpiration = _faker.Date.FutureOffset();

        // Act
        aggregate.ChangeTitle(expectedTitle);
        aggregate.ChangeDescription(expectedDescription);
        aggregate.ExtendExpirationDate(expectedExpiration);

        // Assert
        IDomainEvent[] changes = aggregate.FlushUncommittedChanges();
        changes.Should().HaveCount(3);
        aggregate.AggregateVersion.Should().Be(changes.Length);
        aggregate.GetUncommittedChanges().Should().BeEmpty();
    }

    [Test]
    public void Rehydrate_WhenHistoryIsValid_ShouldRestoreCorrectAggregate()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        ExampleApp.Domain.Posts.Identity.Identity identity = new();
        Title title = Title.Create(_faker.Lorem.Sentence(), true);
        string description = _faker.Lorem.Sentences();
        DateTimeOffset expiration = _faker.Date.FutureOffset();
        Guid imageId = _faker.Random.Guid();
        string imageName = _faker.Lorem.Word();

        HashSet<Image> images = new()
        {
            Image.Create(imageId, _faker.Lorem.Word(), _faker.Random.Bytes(200)),
            Image.Create(_faker.Random.Guid(), _faker.Lorem.Word(), _faker.Random.Bytes(500))
        };

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        ChangeTitleDomainEvent changeTitleEvent = new(aggregateIdentifier, identity, aggregateVersion + 1, title);

        ChangeDescriptionDomainEvent changeDescription =
            new(aggregateIdentifier, identity, aggregateVersion + 2, description);

        ChangeExpirationDateDomainEvent changeExpirationEvent =
            new(aggregateIdentifier, identity, aggregateVersion + 3, expiration);

        AddImagesDomainEvent addImagesEvent = new(aggregateIdentifier, identity, aggregateVersion + 4, images);

        ChangeImageNameDomainEvent changeImageNameEvent =
            new(aggregateIdentifier, identity, aggregateVersion + 5, imageId, imageName);

        IDomainEvent[] history =
        {
            changeTitleEvent, changeDescription, changeExpirationEvent, addImagesEvent, changeImageNameEvent
        };

        // Act
        aggregate.Rehydrate(history);

        // Assert
        aggregate.AggregateVersion.Should().Be(history.Length);
    }

    [Test]
    public void Rehydrate_WhenHistoryIsUnordered_ShouldThrowUnorderedEventsException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        ExampleApp.Domain.Posts.Identity.Identity identity = new();
        Title title = Title.Create(_faker.Lorem.Sentence(), true);
        DateTimeOffset expiration = _faker.Date.FutureOffset();
        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);
        ChangeTitleDomainEvent changeTitleEvent = new(aggregateIdentifier, identity, aggregateVersion + 2, title);
        ChangeExpirationDateDomainEvent changeExpirationEvent =
            new(aggregateIdentifier, identity, aggregateVersion + 1, expiration);
        IDomainEvent[] history = { changeTitleEvent, changeExpirationEvent };

        // Act
        Action action = () => aggregate.Rehydrate(history);

        // Assert
        action.Should().ThrowExactly<UnorderedEventsException>()
            .WithMessage($"The events for this aggregate are not in the expected order ({aggregateIdentifier}).");
    }

    [Test]
    public void Rehydrate_WhenOneOfEventsHaveUnMatchedAggregateId_ShouldThrowUnmatchedDomainEventException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        ExampleApp.Domain.Posts.Identity.Identity identity = new();
        DateTimeOffset expiration = _faker.Date.FutureOffset();
        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);
        ChangeExpirationDateDomainEvent changeExpirationEvent =
            new(_faker.Random.Guid(), identity, aggregateVersion + 2, expiration);
        IDomainEvent[] history = { changeExpirationEvent };

        // Act
        Action action = () => aggregate.Rehydrate(history);

        // Assert
        action.Should().ThrowExactly<UnmatchedDomainEventException>().WithMessage(
            $"Aggregate ({aggregate.GetTypeFullName()}) with identifier: ({aggregateIdentifier}) doesn't match " +
            $"provided domain event ({changeExpirationEvent.GetTypeFullName()}) with expected aggregate identifier: ({changeExpirationEvent.AggregateIdentifier}).");
    }

    [Test]
    public void GetIdentity_WhenIdentityContextMissing_ShouldThrowMissingIdentityContextException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        // Act
        Action action = () => aggregate.GetIdentity();

        // Assert
        action.Should().ThrowExactly<MissingIdentityContextException>().WithMessage(
            $"This aggregate ({aggregate.GetTypeFullName()}) has missing identity context. Consider to build your aggregates via constructor allows to pass an identity parameter.");
    }

    [Test]
    public void
        CreateNewAggregateState_WithOutdatedExpirationDate_ShouldThrowCannotCreateOutdatedPostRuleValidationException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        DateTimeOffset expiration = _faker.Date.PastOffset();
        const int aggregateVersion = 0;
        Title title = Title.Create(_faker.Lorem.Sentence(), true);
        string description = _faker.Lorem.Word();

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        // Act
        Action action = () => aggregate.CreateState(title, description, expiration, null);

        // Assert
        action.Should().ThrowExactly<BusinessRuleValidationException>().WithMessage(
            $"Cannot create ({aggregate.GetTypeFullName()}) with identifier ({aggregateIdentifier}) with outdated expiration date: {expiration}.");
    }

    [Test]
    public async Task
        MutateAggregateState_WithOutdatedExpirationDate_ShouldThrowExpiredPostCannotBeUpdatedRuleValidationException()
    {
        // Arrange
        const int aggregateVersion = 0;
        Guid aggregateIdentifier = _faker.Random.Guid();
        DateTimeOffset expiration = SystemClock.UtcNow.AddMilliseconds(Defaults.DefaultWaitTime);
        Title title = Title.Create(_faker.Lorem.Sentence(), true);
        string description = _faker.Lorem.Sentences();
        Guid imageId = _faker.Random.Guid();
        string imageName = _faker.Lorem.Word();

        HashSet<Image> images = new()
        {
            Image.Create(imageId, _faker.Lorem.Word(), _faker.Random.Bytes(200))
        };

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new ExampleApp.Domain.Posts.Identity.Identity());

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        PostState aggregateState = aggregate.CreateState(Title.Create(_faker.Lorem.Word(), true),
            _faker.Lorem.Sentences(), expiration, images);

        aggregate.RestoreState(aggregateState);

        await Task.Delay(Defaults.DefaultWaitTime + Defaults.DefaultWaitTimeOffset);

        // Act
        Action changeTitleAction = () => aggregate.ChangeTitle(title);
        Action changeDescriptionAction = () => aggregate.ChangeDescription(description);
        Action extendExpirationAction = () => aggregate.ExtendExpirationDate(expiration);
        Action changeImageNameAction = () => aggregate.ChangeImageName(imageId, imageName);
        Action addImagesAction = () => aggregate.AddImages(new HashSet<Image>
        {
            Image.Create(_faker.Random.Guid(), _faker.Lorem.Word(), _faker.Random.Bytes(200))
        });

        // Assert
        string message =
            $"This ({aggregate.GetTypeFullName()}) with identifier ({aggregateIdentifier}) has been already expired. Expiration date time ({expiration})";

        changeTitleAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(message);

        changeDescriptionAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(message);

        extendExpirationAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(message);

        changeImageNameAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(message);

        addImagesAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(message);
    }

    [Test]
    public async Task ExtendExpirationDate_WhenAggregateIsOutdated_ShouldBePossibleToExtendExpirationDate()
    {
        // Arrange
        const int aggregateVersion = 0;
        Guid aggregateIdentifier = _faker.Random.Guid();
        DateTimeOffset expiration = SystemClock.UtcNow.AddMilliseconds(Defaults.DefaultWaitTime);
        DateTimeOffset newExpiration = SystemClock.UtcNow.AddDays(14);

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new ExampleApp.Domain.Posts.Identity.Identity());

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        PostState aggregateState = aggregate.CreateState(Title.Create(_faker.Lorem.Word(), true),
            _faker.Lorem.Sentences(), expiration, null);

        aggregate.RestoreState(aggregateState);

        await Task.Delay(Defaults.DefaultWaitTime + Defaults.DefaultWaitTimeOffset);

        // Act
        Action extendExpirationAction = () => aggregate.ExtendExpirationDate(newExpiration);

        // Assert
        extendExpirationAction.Should().NotThrow();
    }

    [Test]
    public void AddNewImage_WithNotUniqueName_ShouldThrowImageNameMustBeUniqueWithInPostRuleValidationException()
    {
        // Arrange
        const int aggregateVersion = 0;
        Guid aggregateIdentifier = _faker.Random.Guid();
        DateTimeOffset expiration = _faker.Date.FutureOffset();
        Guid imageId = _faker.Random.Guid();
        string imageName = _faker.Lorem.Word();

        HashSet<Image> images = new() { Image.Create(imageId, imageName, _faker.Random.Bytes(200)) };

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new ExampleApp.Domain.Posts.Identity.Identity());

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        PostState aggregateState = aggregate.CreateState(Title.Create(_faker.Lorem.Word(), true),
            _faker.Lorem.Sentences(), expiration, images);

        aggregate.RestoreState(aggregateState);

        Action addImagesAction = () => aggregate.AddImages(new HashSet<Image>
        {
            Image.Create(_faker.Random.Guid(), imageName, _faker.Random.Bytes(200))
        });

        // Assert
        addImagesAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(
                $"Image with name {imageName} has been already added into ({aggregate.GetTypeFullName()}) with identifier ({aggregateIdentifier})");
    }

    [Test]
    public void ChangeImageName_WithNotUniqueName_ShouldThrowImageNameMustBeUniqueWithInPostValidationException()
    {
        // Arrange
        const int aggregateVersion = 0;
        Guid aggregateIdentifier = _faker.Random.Guid();
        DateTimeOffset expiration = SystemClock.UtcNow.AddMilliseconds(Defaults.DefaultWaitTime);
        Guid imageId = _faker.Random.Guid();
        string imageName = _faker.Lorem.Word();
        string duplicateImageName = _faker.Lorem.Word();

        HashSet<Image> images = new()
        {
            Image.Create(imageId, imageName, _faker.Random.Bytes(200)),
            Image.Create(_faker.Random.Guid(), duplicateImageName, _faker.Random.Bytes(200))
        };

        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new ExampleApp.Domain.Posts.Identity.Identity());

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        PostState aggregateState = aggregate.CreateState(Title.Create(_faker.Lorem.Word(), true),
            _faker.Lorem.Sentences(), expiration, images);

        aggregate.RestoreState(aggregateState);

        Action addImagesAction = () => aggregate.ChangeImageName(imageId, duplicateImageName);

        addImagesAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(
                $"Image with name {duplicateImageName} has been already added into ({aggregate.GetTypeFullName()}) with identifier ({aggregateIdentifier})");
    }

    [Test]
    public void SafeDelete_Always_ShouldSetDeleteProperties()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        // Act
        aggregate.SafeDelete();

        // Assert
        aggregate.IsDeleted.Should().BeTrue();
        aggregate.DeletedDate.Should().BeCloseTo(SystemClock.Now, TimeSpan.FromMilliseconds(100));
    }

    [Test]
    public void UnDelete_Always_ShouldSetDeleteProperties()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        // Act
        aggregate.UnDelete();

        // Assert
        aggregate.IsDeleted.Should().BeFalse();
        aggregate.DeletedDate.Should().BeNull();
    }

    private static void AssertState(PostState aggregateState, PostState expectedAggregateState)
    {
        aggregateState.Title.Should().Be(expectedAggregateState.Title);
        aggregateState.Description.Should().Be(expectedAggregateState.Description);
        aggregateState.Expiration.Should().Be(expectedAggregateState.Expiration);
        aggregateState.Images.Should().HaveCount(1);
    }
}