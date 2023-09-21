using System.Collections.Generic;
using System.Linq;
using Bogus;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Persistence.Mappers;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Extensions;
using EssentialFrame.Identity.Interfaces;
using EssentialFrame.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.EventSourcing.Tests.UnitTests.Persistence.Aggregates;

[TestFixture]
public class DomainEventMapperTests
{
    [SetUp]
    public void Setup()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new AppIdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _identityServiceMock.Reset();
        _serializerMock.Reset();
    }

    private readonly Faker _faker = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<ISerializer> _serializerMock = new();
    private readonly IDomainEventMapper<PostIdentifier> _mapper = new DomainEventMapper<PostIdentifier>();

    [Test]
    public void Map_WhenEventIsNotSerialized_ShouldMapDomainEventToDomainEventDataModel()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        TitleChangedDomainEvent changedDomainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Default(_faker.Lorem.Sentence()));

        // Act
        DomainEventDataModel result = _mapper.Map(changedDomainEvent);

        // Assert
        AssertEvent(result, changedDomainEvent);
    }

    [Test]
    public void Map_WhenEventIsSerialized_ShouldMapDomainEventToDomainEventDataModel()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        string serializedEvent = _faker.Random.String(_faker.Random.Int(100, 300));

        TitleChangedDomainEvent changedDomainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Default(_faker.Lorem.Sentence()));

        _serializerMock.Setup(s => s.Serialize<IDomainEvent<PostIdentifier>>(changedDomainEvent))
            .Returns(serializedEvent);

        // Act
        DomainEventDataModel result = _mapper.Map(changedDomainEvent, _serializerMock.Object);

        // Assert
        _serializerMock.Verify(s => s.Serialize<IDomainEvent<PostIdentifier>>(changedDomainEvent), Times.Once);

        AssertEvent(result, changedDomainEvent, _serializerMock.Object);
    }

    [Test]
    public void Map_WhenEventIsNotSerialized_ShouldMapCollectionOfDomainEventsToCollectionOfDomainEventDataModels()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());

        TitleChangedDomainEvent changedDomainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Default(_faker.Lorem.Sentence()));

        // Act
        IEnumerable<DomainEventDataModel> result =
            _mapper.Map(new List<IDomainEvent<PostIdentifier>> { changedDomainEvent });

        // Assert
        result.Should().HaveCount(1);
        DomainEventDataModel domainEventDataModel = result.First();
        AssertEvent(domainEventDataModel, changedDomainEvent);
    }

    [Test]
    public void Map_WhenEventIsSerialized_ShouldMapCollectionOfDomainEventsToCollectionOfDomainEventDataModels()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        string serializedEvent = _faker.Random.String(_faker.Random.Int(100, 300));

        TitleChangedDomainEvent changedDomainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Default(_faker.Lorem.Sentence()));

        _serializerMock.Setup(s => s.Serialize<IDomainEvent<PostIdentifier>>(changedDomainEvent))
            .Returns(serializedEvent);

        // Act
        IEnumerable<DomainEventDataModel> result = _mapper.Map(
            new List<IDomainEvent<PostIdentifier>> { changedDomainEvent },
            _serializerMock.Object);

        // Assert
        _serializerMock.Verify(s => s.Serialize<IDomainEvent<PostIdentifier>>(changedDomainEvent), Times.Once);

        result.Should().HaveCount(1);
        DomainEventDataModel domainEventDataModel = result.First();
        AssertEvent(domainEventDataModel, changedDomainEvent, _serializerMock.Object);
    }

    [Test]
    public void Map_WhenEventIsNotSerialized_ShouldMapDomainEventDataModelToDomainEvent()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());

        TitleChangedDomainEvent changedDomainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Default(_faker.Lorem.Sentence()));

        DomainEventDataModel domainEventDataModel = new()
        {
            AggregateIdentifier = changedDomainEvent.AggregateIdentifier.Value,
            AggregateVersion = changedDomainEvent.AggregateVersion,
            EventIdentifier = changedDomainEvent.EventIdentifier,
            EventType = changedDomainEvent.GetTypeFullName(),
            EventClass = changedDomainEvent.GetClassName(),
            DomainEvent = changedDomainEvent,
            CreatedAt = changedDomainEvent.EventTime
        };

        // Act
        IDomainEvent<PostIdentifier> result = _mapper.Map(domainEventDataModel);

        // Assert
        AssertEvent(domainEventDataModel, result);
    }

    [Test]
    public void Map_WhenEventIsSerialized_ShouldMapDomainEventDataModelToDomainEvent()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        string serializedEvent = _faker.Random.String(_faker.Random.Int(100, 300));

        TitleChangedDomainEvent changedDomainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Default(_faker.Lorem.Sentence()));

        DomainEventDataModel domainEventDataModel = new()
        {
            AggregateIdentifier = changedDomainEvent.AggregateIdentifier.Value,
            AggregateVersion = changedDomainEvent.AggregateVersion,
            EventIdentifier = changedDomainEvent.EventIdentifier,
            EventType = changedDomainEvent.GetTypeFullName(),
            EventClass = changedDomainEvent.GetClassName(),
            DomainEvent = serializedEvent,
            CreatedAt = changedDomainEvent.EventTime
        };

        _serializerMock.Setup(s => s.Serialize(It.IsAny<IDomainEvent<PostIdentifier>>())).Returns(serializedEvent);

        _serializerMock.Setup(s =>
            s.Deserialize<IDomainEvent<PostIdentifier>>(domainEventDataModel.DomainEvent as string,
                changedDomainEvent.GetType())).Returns(changedDomainEvent);

        // Act
        IDomainEvent<PostIdentifier> result = _mapper.Map(domainEventDataModel, _serializerMock.Object);

        // Assert
        _serializerMock.Verify(
            s => s.Deserialize<IDomainEvent<PostIdentifier>>(domainEventDataModel.DomainEvent as string,
                changedDomainEvent.GetType()), Times.Once);

        AssertEvent(domainEventDataModel, result, _serializerMock.Object);
    }

    [Test]
    public void Map_WhenEventIsNotSerialized_ShouldMapCollectionOfDomainEventDataModelsToCollectionOfDomainEvents()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());

        TitleChangedDomainEvent changedDomainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Default(_faker.Lorem.Sentence()));

        DomainEventDataModel domainEventDataModel = new()
        {
            AggregateIdentifier = changedDomainEvent.AggregateIdentifier.Value,
            AggregateVersion = changedDomainEvent.AggregateVersion,
            EventIdentifier = changedDomainEvent.EventIdentifier,
            EventType = changedDomainEvent.GetTypeFullName(),
            EventClass = changedDomainEvent.GetClassName(),
            DomainEvent = changedDomainEvent,
            CreatedAt = changedDomainEvent.EventTime
        };

        // Act
        IEnumerable<IDomainEvent<PostIdentifier>> result =
            _mapper.Map(new List<DomainEventDataModel> { domainEventDataModel });

        // Assert
        result.Should().HaveCount(1);
        IDomainEvent<PostIdentifier> domainEventResult = result.First();
        AssertEvent(domainEventDataModel, domainEventResult);
    }

    [Test]
    public void Map_WhenEventIsSerialized_ShouldMapCollectionOfDomainEventDataModelsToCollectionOfDomainEvents()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        string serializedEvent = _faker.Random.String(_faker.Random.Int(100, 300));

        TitleChangedDomainEvent changedDomainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Default(_faker.Lorem.Sentence()));

        DomainEventDataModel domainEventDataModel = new()
        {
            AggregateIdentifier = changedDomainEvent.AggregateIdentifier.Value,
            AggregateVersion = changedDomainEvent.AggregateVersion,
            EventIdentifier = changedDomainEvent.EventIdentifier,
            EventType = changedDomainEvent.GetTypeFullName(),
            EventClass = changedDomainEvent.GetClassName(),
            DomainEvent = serializedEvent,
            CreatedAt = changedDomainEvent.EventTime
        };

        _serializerMock.Setup(s => s.Serialize(It.IsAny<IDomainEvent<PostIdentifier>>())).Returns(serializedEvent);

        _serializerMock.Setup(s =>
            s.Deserialize<IDomainEvent<PostIdentifier>>(domainEventDataModel.DomainEvent as string,
                changedDomainEvent.GetType())).Returns(changedDomainEvent);

        // Act
        IEnumerable<IDomainEvent<PostIdentifier>> result = _mapper.Map(
            new List<DomainEventDataModel> { domainEventDataModel }, _serializerMock.Object);

        // Assert
        _serializerMock.Verify(
            s => s.Deserialize<IDomainEvent<PostIdentifier>>(domainEventDataModel.DomainEvent as string,
                changedDomainEvent.GetType()), Times.Once);

        result.Should().HaveCount(1);
        IDomainEvent<PostIdentifier> domainEventResult = result.First();
        AssertEvent(domainEventDataModel, domainEventResult, _serializerMock.Object);
    }

    private static void AssertEvent(DomainEventDataModel domainEventDataModel, IDomainEvent<PostIdentifier> domainEvent,
        ISerializer serializer = null)
    {
        domainEventDataModel.AggregateIdentifier.Should().Be(domainEvent.AggregateIdentifier.Value);
        domainEventDataModel.AggregateVersion.Should().Be(domainEvent.AggregateVersion);
        domainEventDataModel.EventIdentifier.Should().Be(domainEvent.EventIdentifier);
        domainEventDataModel.EventType.Should().Be(domainEvent.GetTypeFullName());
        domainEventDataModel.EventClass.Should().Be(domainEvent.GetClassName());

        if (serializer is null)
        {
            domainEventDataModel.DomainEvent.Should().BeEquivalentTo(domainEvent);
        }
        else
        {
            domainEventDataModel.DomainEvent.Should().BeEquivalentTo(serializer.Serialize(domainEvent));
        }

        domainEventDataModel.CreatedAt.Should().Be(domainEvent.EventTime);
    }
}