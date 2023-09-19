using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.EventSourcing.Core.Factories;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates.Rules;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Extensions;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.Tests.Utils;
using EssentialFrame.Time;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.EventSourcing.Tests.UnitTests.Core.Aggregates;

[TestFixture]
public sealed class EventSourcingAggregateRootTests
{
    private readonly Faker _faker = new();
    private readonly Mock<ISerializer> _serializerMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();


    [SetUp]
    public void SetUp()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _identityServiceMock.Reset();
    }

    [Test]
    public void CreateState_Always_ShouldReturnSpecificType()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Number();

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        // Act
        PostState state = aggregate.CreateState();

        // Assert
        state.Should().BeOfType(typeof(PostState));
    }

    [Test]
    public void RestoreState_Always_ShouldAssignCorrectState()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Number();
        string serializedState = _faker.Lorem.Sentence();

        Post expectedAggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        expectedAggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()),
            new HashSet<Image>
            {
                Image.Create(_faker.Random.Guid(),
                    Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                    BytesContent.Create(_faker.Random.Bytes(300)))
            }, _identityServiceMock.Object.GetCurrent());

        PostState expectedAggregateState = expectedAggregate.State as PostState;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

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
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null, _identityServiceMock.Object.GetCurrent());

        Title expectedTitle = Title.Default(_faker.Lorem.Sentence());
        Description expectedDescription = Description.Create(_faker.Lorem.Sentences());
        Date expectedExpiration = Date.Create(_faker.Date.FutureOffset());

        // Act
        aggregate.ChangeTitle(expectedTitle, _identityServiceMock.Object.GetCurrent());

        // Assert
        PostState aggregateState = (PostState)aggregate.State;
        aggregateState.Title.Should().Be(expectedTitle);

        // Act
        aggregate.ChangeDescription(expectedDescription, _identityServiceMock.Object.GetCurrent());

        // Assert
        aggregateState = (PostState)aggregate.State;
        aggregateState.Description.Should().Be(expectedDescription);

        // Act
        aggregate.ExtendExpirationDate(expectedExpiration, _identityServiceMock.Object.GetCurrent());

        // Assert
        aggregateState = (PostState)aggregate.State;
        aggregateState.Expiration.Should().Be(expectedExpiration);
    }

    [Test]
    public void GetUncommittedChanges_Always_ShouldGetAllChangesFromAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null, _identityServiceMock.Object.GetCurrent());

        Title expectedTitle = Title.Default(_faker.Lorem.Sentence());
        Description expectedDescription = Description.Create(_faker.Lorem.Sentences());
        Date expectedExpiration = Date.Create(_faker.Date.FutureOffset());

        // Act
        aggregate.ChangeTitle(expectedTitle, _identityServiceMock.Object.GetCurrent());
        aggregate.ChangeDescription(expectedDescription, _identityServiceMock.Object.GetCurrent());
        aggregate.ExtendExpirationDate(expectedExpiration, _identityServiceMock.Object.GetCurrent());

        // Assert
        IDomainEvent<PostIdentifier>[] changes = aggregate.GetUncommittedChanges();
        changes.Should().HaveCount(4);
    }

    [Test]
    public void FlushUncommittedChanges_Always_ShouldIncreaseAggregateVersion()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        Mock<IDomainEventsPublisher<PostIdentifier>> domainEventsPublisherMock = new();
        const int aggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null, _identityServiceMock.Object.GetCurrent());

        Title expectedTitle = Title.Default(_faker.Lorem.Sentence());
        Description expectedDescription = Description.Create(_faker.Lorem.Sentences());
        Date expectedExpiration = Date.Create(_faker.Date.FutureOffset());

        // Act
        aggregate.ChangeTitle(expectedTitle, _identityServiceMock.Object.GetCurrent());
        aggregate.ChangeDescription(expectedDescription, _identityServiceMock.Object.GetCurrent());
        aggregate.ExtendExpirationDate(expectedExpiration, _identityServiceMock.Object.GetCurrent());

        // Assert
        IDomainEvent<PostIdentifier>[] changes = aggregate.FlushUncommittedChanges(domainEventsPublisherMock.Object);
        changes.Should().HaveCount(4);
        aggregate.AggregateVersion.Should().Be(changes.Length);
        aggregate.GetUncommittedChanges().Should().BeEmpty();
    }

    [Test]
    public void Rehydrate_WhenHistoryIsValid_ShouldRestoreCorrectAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        Guid imageId = _faker.Random.Guid();
        const int aggregateVersion = 0;
        IIdentityContext identityContext = _identityServiceMock.Object.GetCurrent();
        Title title = Title.Default(_faker.Lorem.Sentence());
        Description description = Description.Create(_faker.Lorem.Sentences());
        Date expiration = Date.Create(_faker.Date.FutureOffset());
        Name imageName = Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)));

        HashSet<Image> images = new()
        {
            Image.Create(imageId, Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                BytesContent.Create(_faker.Random.Bytes(200))),
            Image.Create(_faker.Random.Guid(),
                Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                BytesContent.Create(_faker.Random.Bytes(500)))
        };

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        CreateNewPostDomainEvent createNewPostEvent = new(aggregateIdentifier, identityContext, aggregateVersion + 1,
            Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null);

        ChangeTitleDomainEvent changeTitleEvent =
            new(aggregateIdentifier, identityContext, aggregateVersion + 2, title);

        ChangeDescriptionDomainEvent changeDescription =
            new(aggregateIdentifier, identityContext, aggregateVersion + 3, description);

        ChangeExpirationDateDomainEvent changeExpirationEvent =
            new(aggregateIdentifier, identityContext, aggregateVersion + 4, expiration);

        AddImagesDomainEvent addImagesEvent = new(aggregateIdentifier, identityContext, aggregateVersion + 5, images);

        ChangeImageNameDomainEvent changeImageNameEvent =
            new(aggregateIdentifier, identityContext, aggregateVersion + 6, imageId, imageName);

        IDomainEvent<PostIdentifier>[] history =
        {
            createNewPostEvent, changeTitleEvent, changeDescription, changeExpirationEvent, addImagesEvent,
            changeImageNameEvent
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
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;
        IIdentityContext identityContext = _identityServiceMock.Object.GetCurrent();
        Title title = Title.Default(_faker.Lorem.Sentence());
        Date expiration = Date.Create(_faker.Date.FutureOffset());

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        ChangeTitleDomainEvent changeTitleEvent =
            new(aggregateIdentifier, identityContext, aggregateVersion + 2, title);

        ChangeExpirationDateDomainEvent changeExpirationEvent =
            new(aggregateIdentifier, identityContext, aggregateVersion + 1, expiration);

        IDomainEvent<PostIdentifier>[] history = { changeTitleEvent, changeExpirationEvent };

        // Act
        Action action = () => aggregate.Rehydrate(history);

        // Assert
        action.Should().ThrowExactly<UnorderedEventsException>()
            .WithMessage($"The events for this aggregate are not in the expected order ({aggregateIdentifier})");
    }

    [Test]
    public void Rehydrate_WhenOneOfEventsHaveUnMatchedAggregateId_ShouldThrowUnmatchedDomainEventException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;
        IIdentityContext identityContext = _identityServiceMock.Object.GetCurrent();
        Date expiration = Date.Create(_faker.Date.FutureOffset());

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        ChangeExpirationDateDomainEvent changeExpirationEvent = new(PostIdentifier.New(_faker.Random.Guid()),
            identityContext, aggregateVersion + 2, expiration);

        IDomainEvent<PostIdentifier>[] history = { changeExpirationEvent };

        // Act
        Action action = () => aggregate.Rehydrate(history);

        // Assert
        action.Should().ThrowExactly<UnmatchedDomainEventException>().WithMessage(
            $"Aggregate ({aggregate.GetTypeFullName()}) with identifier: ({aggregateIdentifier}) doesn't match " +
            $"provided domain event ({changeExpirationEvent.GetTypeFullName()}) with expected aggregate identifier: ({changeExpirationEvent.AggregateIdentifier})");
    }

    [Test]
    public void
        CreateNewAggregateState_WithOutdatedExpirationDate_ShouldThrowCannotCreateOutdatedPostRuleValidationException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        DateTimeOffset expiration = _faker.Date.PastOffset();
        const int aggregateVersion = 0;
        Title title = Title.Default(_faker.Lorem.Sentence());
        Description description = Description.Create(_faker.Lorem.Sentences());

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        // Act
        Action action = () => aggregate.Create(title, description, Date.Create(expiration), null,
            _identityServiceMock.Object.GetCurrent());

        // Assert
        string message =
            $"Cannot create ({aggregate.GetTypeFullName()}) with identifier ({aggregateIdentifier}) with outdated expiration date: {expiration}";

        action.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerExceptionExactly<BusinessRuleValidationException>().WithMessage(message).Where(x =>
                x.BrokenRule is CannotCreateOutdatedPostRule &&
                x.BrokenRule.Parameters.ContainsKey(nameof(PostState.Expiration)) && x.Details == message);
    }

    [Test]
    public async Task
        MutateAggregateState_WithOutdatedExpirationDate_ShouldThrowExpiredPostCannotBeUpdatedRuleValidationException()
    {
        // Arrange
        const int aggregateVersion = 0;
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        Guid imageId = _faker.Random.Guid();
        Date expiration = Date.Create(SystemClock.UtcNow.AddMilliseconds(Defaults.DefaultWaitTime));
        Title title = Title.Default(_faker.Lorem.Sentence());
        Description description = Description.Create(_faker.Lorem.Sentences());
        Name imageName = Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)));

        HashSet<Image> images = new()
        {
            Image.Create(imageId, Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                BytesContent.Create(_faker.Random.Bytes(200)))
        };


        Post expectedAggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        expectedAggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            expiration, images, _identityServiceMock.Object.GetCurrent());

        PostState expectedAggregateState = expectedAggregate.State as PostState;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        aggregate.RestoreState(expectedAggregateState);

        await Task.Delay(Defaults.DefaultWaitTime + Defaults.DefaultWaitTimeOffset);

        // Act
        Action changeTitleAction = () => aggregate.ChangeTitle(title, _identityServiceMock.Object.GetCurrent());
        Action changeDescriptionAction = () =>
            aggregate.ChangeDescription(description, _identityServiceMock.Object.GetCurrent());
        Action extendExpirationAction = () =>
            aggregate.ExtendExpirationDate(expiration, _identityServiceMock.Object.GetCurrent());
        Action changeImageNameAction = () =>
            aggregate.ChangeImageName(imageId, imageName, _identityServiceMock.Object.GetCurrent());

        Action addImagesAction = () =>
            aggregate.AddImages(
                new HashSet<Image>
                {
                    Image.Create(_faker.Random.Guid(),
                        Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                        BytesContent.Create(_faker.Random.Bytes(200)))
                }, _identityServiceMock.Object.GetCurrent());

        // Assert
        string message =
            $"This ({aggregate.GetTypeFullName()}) with identifier ({aggregateIdentifier}) has been already expired. Expiration date time ({expiration})";

        changeTitleAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(message).Where(x =>
                x.BrokenRule is ExpiredPostCannotBeUpdatedRule &&
                x.BrokenRule.Parameters.ContainsKey(nameof(PostState.Expiration)) && x.Details == message);

        changeDescriptionAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(message).Where(x =>
                x.BrokenRule is ExpiredPostCannotBeUpdatedRule &&
                x.BrokenRule.Parameters.ContainsKey(nameof(PostState.Expiration)) && x.Details == message);

        extendExpirationAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(message).Where(x =>
                x.BrokenRule is ExpiredPostCannotBeUpdatedRule &&
                x.BrokenRule.Parameters.ContainsKey(nameof(PostState.Expiration)) && x.Details == message);

        changeImageNameAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(message).Where(x =>
                x.BrokenRule is ExpiredPostCannotBeUpdatedRule &&
                x.BrokenRule.Parameters.ContainsKey(nameof(PostState.Expiration)) && x.Details == message);

        addImagesAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(message).Where(x =>
                x.BrokenRule is ExpiredPostCannotBeUpdatedRule &&
                x.BrokenRule.Parameters.ContainsKey(nameof(PostState.Expiration)) && x.Details == message);
    }

    [Test]
    public async Task ExtendExpirationDate_WhenAggregateIsOutdated_ShouldBePossibleToExtendExpirationDate()
    {
        // Arrange
        const int aggregateVersion = 0;
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        Date expiration = Date.Create(SystemClock.UtcNow.AddMilliseconds(Defaults.DefaultWaitTime));
        Date newExpiration = Date.Create(SystemClock.UtcNow.AddDays(14));

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            expiration, null, _identityServiceMock.Object.GetCurrent());

        await Task.Delay(Defaults.DefaultWaitTime + Defaults.DefaultWaitTimeOffset);

        // Act
        Action extendExpirationAction = () =>
            aggregate.ExtendExpirationDate(newExpiration, _identityServiceMock.Object.GetCurrent());

        // Assert
        extendExpirationAction.Should().NotThrow();
    }

    [Test]
    public void AddNewImage_WithNotUniqueName_ShouldThrowImageNameMustBeUniqueWithInPostRuleValidationException()
    {
        // Arrange
        const int aggregateVersion = 0;
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        Guid imageId = _faker.Random.Guid();
        Date expiration = Date.Create(_faker.Date.FutureOffset());
        Name imageName = Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)));

        HashSet<Image> images =
            new() { Image.Create(imageId, imageName, BytesContent.Create(_faker.Random.Bytes(200))) };

        Post expectedAggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        expectedAggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            expiration, images, _identityServiceMock.Object.GetCurrent());

        PostState expectedAggregateState = expectedAggregate.State as PostState;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        aggregate.RestoreState(expectedAggregateState);

        Action addImagesAction = () =>
            aggregate.AddImages(
                new HashSet<Image>
                {
                    Image.Create(_faker.Random.Guid(), imageName, BytesContent.Create(_faker.Random.Bytes(200)))
                }, _identityServiceMock.Object.GetCurrent());

        // Assert
        addImagesAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(
                $"Image with name {imageName.Value} has been already added into ({aggregate.GetTypeFullName()}) with identifier ({aggregateIdentifier})");
    }

    [Test]
    public void ChangeImageName_WithNotUniqueName_ShouldThrowImageNameMustBeUniqueWithInPostValidationException()
    {
        // Arrange
        const int aggregateVersion = 0;
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        Guid imageId = _faker.Random.Guid();
        Date expiration = Date.Create(SystemClock.UtcNow.AddMilliseconds(Defaults.DefaultWaitTime));
        Name imageName = Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)));
        Name duplicateImageName = Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)));

        HashSet<Image> images = new()
        {
            Image.Create(imageId, imageName, BytesContent.Create(_faker.Random.Bytes(200))),
            Image.Create(_faker.Random.Guid(), duplicateImageName, BytesContent.Create(_faker.Random.Bytes(200)))
        };

        Post expectedAggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        expectedAggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            expiration, images, _identityServiceMock.Object.GetCurrent());

        PostState expectedAggregateState = expectedAggregate.State as PostState;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        aggregate.RestoreState(expectedAggregateState);

        Action addImagesAction = () =>
            aggregate.ChangeImageName(imageId, duplicateImageName, _identityServiceMock.Object.GetCurrent());

        addImagesAction.Should().ThrowExactly<TargetInvocationException>()
            .WithInnerException<BusinessRuleValidationException>().WithMessage(
                $"Image with name {duplicateImageName.Value} has been already added into ({aggregate.GetTypeFullName()}) with identifier ({aggregateIdentifier})");
    }

    [Test]
    public void SafeDelete_Always_ShouldSetDeleteProperties()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

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
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

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