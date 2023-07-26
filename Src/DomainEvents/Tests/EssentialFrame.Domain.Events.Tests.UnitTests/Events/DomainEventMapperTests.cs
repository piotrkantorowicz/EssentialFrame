using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects;
using EssentialFrame.ExampleApp.Identity;
using EssentialFrame.Extensions;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Events;

[TestFixture]
public class DomainEventMapperTests
{
    [SetUp]
    public void Setup()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());
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

    [Test]
    public void Map_WhenEventIsNotSerialized_ShouldMapDomainEventToDomainEventDataModel()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        ChangeTitleDomainEvent domainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Create(_faker.Lorem.Sentence(), _faker.Random.Bool()));
        IDomainEventMapper mapper = new DomainEventMapper();

        // Act
        DomainEventDataModel result = mapper.Map(domainEvent);

        // Assert
        AssertEvent(result, domainEvent);
    }

    [Test]
    public void Map_WhenEventIsSerialized_ShouldMapDomainEventToDomainEventDataModel()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        string serializedEvent = _faker.Random.String(_faker.Random.Int(100, 300));
        ChangeTitleDomainEvent domainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Create(_faker.Lorem.Sentence(), _faker.Random.Bool()));
        _serializerMock.Setup(s => s.Serialize<IDomainEvent>(domainEvent)).Returns(serializedEvent);
        IDomainEventMapper mapper = new DomainEventMapper();

        // Act
        DomainEventDataModel result = mapper.Map(domainEvent, _serializerMock.Object);

        // Assert
        _serializerMock.Verify(s => s.Serialize<IDomainEvent>(domainEvent), Times.Once);

        AssertEvent(result, domainEvent, _serializerMock.Object);
    }

    [Test]
    public void Map_WhenEventIsNotSerialized_ShouldMapCollectionOfDomainEventsToCollectionOfDomainEventDataModels()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        ChangeTitleDomainEvent domainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Create(_faker.Lorem.Sentence(), _faker.Random.Bool()));
        IDomainEventMapper mapper = new DomainEventMapper();

        // Act
        IEnumerable<DomainEventDataModel> result = mapper.Map(new List<IDomainEvent> { domainEvent });

        // Assert
        result.Should().HaveCount(1);
        DomainEventDataModel domainEventDataModel = result.First();
        AssertEvent(domainEventDataModel, domainEvent);
    }

    [Test]
    public void Map_WhenEventIsSerialized_ShouldMapCollectionOfDomainEventsToCollectionOfDomainEventDataModels()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        string serializedEvent = _faker.Random.String(_faker.Random.Int(100, 300));
        ChangeTitleDomainEvent domainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Create(_faker.Lorem.Sentence(), _faker.Random.Bool()));
        _serializerMock.Setup(s => s.Serialize<IDomainEvent>(domainEvent)).Returns(serializedEvent);
        IDomainEventMapper mapper = new DomainEventMapper();

        // Act
        IEnumerable<DomainEventDataModel> result = mapper.Map(new List<IDomainEvent> { domainEvent },
            _serializerMock.Object);

        // Assert
        _serializerMock.Verify(s => s.Serialize<IDomainEvent>(domainEvent), Times.Once);

        result.Should().HaveCount(1);
        DomainEventDataModel domainEventDataModel = result.First();
        AssertEvent(domainEventDataModel, domainEvent, _serializerMock.Object);
    }

    [Test]
    public void Map_WhenEventIsNotSerialized_ShouldMapDomainEventDataModelToDomainEvent()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();

        ChangeTitleDomainEvent domainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Create(_faker.Lorem.Sentence(), _faker.Random.Bool()));

        DomainEventDataModel domainEventDataModel = new()
        {
            AggregateIdentifier = domainEvent.AggregateIdentifier,
            AggregateVersion = domainEvent.AggregateVersion,
            EventIdentifier = domainEvent.EventIdentifier,
            EventType = domainEvent.GetTypeFullName(),
            EventClass = domainEvent.GetClassName(),
            DomainEvent = domainEvent,
            CreatedAt = domainEvent.EventTime
        };

        IDomainEventMapper mapper = new DomainEventMapper();

        // Act
        IDomainEvent result = mapper.Map(domainEventDataModel);

        // Assert
        AssertEvent(domainEventDataModel, result);
    }

    [Test]
    public void Map_WhenEventIsSerialized_ShouldMapDomainEventDataModelToDomainEvent()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        string serializedEvent = _faker.Random.String(_faker.Random.Int(100, 300));
        ChangeTitleDomainEvent domainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Create(_faker.Lorem.Sentence(), _faker.Random.Bool()));

        DomainEventDataModel domainEventDataModel = new()
        {
            AggregateIdentifier = domainEvent.AggregateIdentifier,
            AggregateVersion = domainEvent.AggregateVersion,
            EventIdentifier = domainEvent.EventIdentifier,
            EventType = domainEvent.GetTypeFullName(),
            EventClass = domainEvent.GetClassName(),
            DomainEvent = serializedEvent,
            CreatedAt = domainEvent.EventTime
        };

        _serializerMock.Setup(s => s.Serialize(It.IsAny<IDomainEvent>())).Returns(serializedEvent);

        _serializerMock
            .Setup(s => s.Deserialize<IDomainEvent>(domainEventDataModel.DomainEvent as string, domainEvent.GetType()))
            .Returns(domainEvent);

        IDomainEventMapper mapper = new DomainEventMapper();

        // Act
        IDomainEvent result = mapper.Map(domainEventDataModel, _serializerMock.Object);

        // Assert
        _serializerMock.Verify(
            s => s.Deserialize<IDomainEvent>(domainEventDataModel.DomainEvent as string, domainEvent.GetType()),
            Times.Once);

        AssertEvent(domainEventDataModel, result, _serializerMock.Object);
    }

    [Test]
    public void Map_WhenEventIsNotSerialized_ShouldMapCollectionOfDomainEventDataModelsToCollectionOfDomainEvents()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();

        ChangeTitleDomainEvent domainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Create(_faker.Lorem.Sentence(), _faker.Random.Bool()));

        DomainEventDataModel domainEventDataModel = new()
        {
            AggregateIdentifier = domainEvent.AggregateIdentifier,
            AggregateVersion = domainEvent.AggregateVersion,
            EventIdentifier = domainEvent.EventIdentifier,
            EventType = domainEvent.GetTypeFullName(),
            EventClass = domainEvent.GetClassName(),
            DomainEvent = domainEvent,
            CreatedAt = domainEvent.EventTime
        };

        IDomainEventMapper mapper = new DomainEventMapper();

        // Act
        IEnumerable<IDomainEvent> result = mapper.Map(new List<DomainEventDataModel> { domainEventDataModel });

        // Assert
        result.Should().HaveCount(1);
        IDomainEvent domainEventResult = result.First();
        AssertEvent(domainEventDataModel, domainEventResult);
    }

    [Test]
    public void Map_WhenEventIsSerialized_ShouldMapCollectionOfDomainEventDataModelsToCollectionOfDomainEvents()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        string serializedEvent = _faker.Random.String(_faker.Random.Int(100, 300));
        ChangeTitleDomainEvent domainEvent = new(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
            Title.Create(_faker.Lorem.Sentence(), _faker.Random.Bool()));

        DomainEventDataModel domainEventDataModel = new()
        {
            AggregateIdentifier = domainEvent.AggregateIdentifier,
            AggregateVersion = domainEvent.AggregateVersion,
            EventIdentifier = domainEvent.EventIdentifier,
            EventType = domainEvent.GetTypeFullName(),
            EventClass = domainEvent.GetClassName(),
            DomainEvent = serializedEvent,
            CreatedAt = domainEvent.EventTime
        };

        _serializerMock.Setup(s => s.Serialize(It.IsAny<IDomainEvent>())).Returns(serializedEvent);

        _serializerMock
            .Setup(s => s.Deserialize<IDomainEvent>(domainEventDataModel.DomainEvent as string, domainEvent.GetType()))
            .Returns(domainEvent);

        IDomainEventMapper mapper = new DomainEventMapper();

        // Act
        IEnumerable<IDomainEvent> result = mapper.Map(new List<DomainEventDataModel> { domainEventDataModel },
            _serializerMock.Object);

        // Assert
        _serializerMock.Verify(
            s => s.Deserialize<IDomainEvent>(domainEventDataModel.DomainEvent as string, domainEvent.GetType()),
            Times.Once);

        result.Should().HaveCount(1);
        IDomainEvent domainEventResult = result.First();
        AssertEvent(domainEventDataModel, domainEventResult, _serializerMock.Object);
    }

    private static void AssertEvent(DomainEventDataModel domainEventDataModel, IDomainEvent domainEvent,
        ISerializer serializer = null)
    {
        domainEventDataModel.AggregateIdentifier.Should().Be(domainEvent.AggregateIdentifier);
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