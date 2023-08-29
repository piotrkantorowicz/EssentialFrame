using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Core.Snapshots.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Models;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Services;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Services.Interfaces;
using EssentialFrame.Domain.Factories;
using EssentialFrame.Domain.Snapshots;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Persistence.Snapshots;

[TestFixture]
public class SnapshotRepositoryTests
{
    private readonly Mock<IAggregateRepository> _aggregateRepositoryMock = new();
    private readonly Mock<IAggregateStore> _aggregateStoreMock = new();
    private readonly Mock<ICache<Guid, AggregateRoot>> _snapshotsCacheMock = new();
    private readonly Mock<IDomainEventMapper> _domainEventMapperMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<ISerializer> _serializerMock = new();
    private readonly Mock<ISnapshotMapper> _snapshotMapperMock = new();
    private readonly Mock<ISnapshotStore> _snapshotStoreMock = new();
    private readonly Mock<ISnapshotStrategy> _snapshotStrategyMock = new();
    private readonly Faker _faker = new();

    [SetUp]
    public void SetUp()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _aggregateRepositoryMock.Reset();
        _aggregateStoreMock.Reset();
        _snapshotsCacheMock.Reset();
        _domainEventMapperMock.Reset();
        _identityServiceMock.Reset();
        _serializerMock.Reset();
        _snapshotMapperMock.Reset();
        _snapshotStoreMock.Reset();
        _snapshotStrategyMock.Reset();
    }

    [Test]
    public async Task Get_WhenAggregateIsInCache_ShouldReturnAggregateFromSnapshotCache()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        _snapshotsCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(aggregate);

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        Post result = await snapshotRepository.GetAsync<Post>(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIsInCache_ShouldReturnAggregateFromSnapshotCache()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Number();

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        _snapshotsCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(aggregate);

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        Post result = await snapshotRepository.GetAsync<Post>(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public void Get_WhenAggregateIsNotInCacheButExistsInDatabase_ShouldReturnAggregateFromSnapshotDatabase()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> eventDataModels = GenerateDomainEventDataModelsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier, AggregateVersion = events.Count, AggregateState = aggregate.State
        };

        Snapshot snapshot = new(snapshotDataModel.AggregateIdentifier, snapshotDataModel.AggregateVersion,
            snapshotDataModel.AggregateState);

        _snapshotStoreMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        _snapshotMapperMock.Setup(x => x.Map(snapshotDataModel)).Returns(snapshot);
        _aggregateStoreMock.Setup(x => x.Get(aggregateIdentifier, snapshot.AggregateVersion)).Returns(eventDataModels);
        _domainEventMapperMock.Setup(x => x.Map(eventDataModels)).Returns(events);

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        Post result = snapshotRepository.Get<Post>(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIsNotInCacheButExistsInDatabase_ShouldReturnAggregateFromSnapshotDatabase()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> eventDataModels = GenerateDomainEventDataModelsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier, AggregateVersion = events.Count, AggregateState = aggregate.State
        };

        Snapshot snapshot = new(snapshotDataModel.AggregateIdentifier, snapshotDataModel.AggregateVersion,
            snapshotDataModel.AggregateState);

        _snapshotStoreMock.Setup(x => x.GetAsync(aggregateIdentifier, default)).ReturnsAsync(snapshotDataModel);
        _snapshotMapperMock.Setup(x => x.Map(snapshotDataModel)).Returns(snapshot);

        _aggregateStoreMock.Setup(x => x.GetAsync(aggregateIdentifier, snapshot.AggregateVersion, default))
            .ReturnsAsync(eventDataModels);

        _domainEventMapperMock.Setup(x => x.Map(eventDataModels)).Returns(events);

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        Post result = await snapshotRepository.GetAsync<Post>(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public void Get_WhenAggregateIsNotInCacheAndSnapshotDatabase_ShouldReturnAggregateFromAggregateDatabase()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotStoreMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);

        _aggregateRepositoryMock.Setup(x => x.Get<Post>(aggregateIdentifier)).Returns(aggregate);

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        Post result = snapshotRepository.Get<Post>(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIsNotInCacheAndSnapshotDatabase_ShouldReturnAggregateFromAggregateDatabase()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotStoreMock.Setup(x => x.GetAsync(aggregateIdentifier, default)).ReturnsAsync((SnapshotDataModel)null);

        _aggregateRepositoryMock.Setup(x => x.GetAsync<Post>(aggregateIdentifier, default)).ReturnsAsync(aggregate);

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        Post result = await snapshotRepository.GetAsync<Post>(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public void Save_WhenVersionHasNotBeenProvided_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        int timeout = _faker.Random.Int(1, 1000);

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotsCacheMock.Setup(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true));
        _aggregateRepositoryMock.Setup(x => x.Save(aggregate, null));

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        snapshotRepository.Save(aggregate, timeout: timeout);

        // Assert
        _snapshotsCacheMock.Verify(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true), Times.Once);
        _aggregateRepositoryMock.Verify(x => x.Save(aggregate, null), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenVersionHasNotBeenProvided_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        int timeout = _faker.Random.Int(1, 1000);

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotsCacheMock.Setup(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true));
        _aggregateRepositoryMock.Setup(x => x.SaveAsync(aggregate, null, default));

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        await snapshotRepository.SaveAsync(aggregate, timeout: timeout);

        // Assert
        _snapshotsCacheMock.Verify(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true), Times.Once);
        _aggregateRepositoryMock.Verify(x => x.SaveAsync(aggregate, null, default), Times.Once);
    }

    [Test]
    public void Save_WhenVersionHasBeenProvided_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        int timeout = _faker.Random.Int(1, 1000);

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotsCacheMock.Setup(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true));
        _aggregateRepositoryMock.Setup(x => x.Save(aggregate, aggregate.AggregateVersion));

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        snapshotRepository.Save(aggregate, aggregate.AggregateVersion, timeout);

        // Assert
        _snapshotsCacheMock.Verify(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true), Times.Once);
        _aggregateRepositoryMock.Verify(x => x.Save(aggregate, aggregate.AggregateVersion), Times.Once);
    }


    [Test]
    public void Save_WithoutTimeout_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        int timeout = _faker.Random.Int(1, 1000);

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotsCacheMock.Setup(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true));
        _aggregateRepositoryMock.Setup(x => x.Save(aggregate, aggregate.AggregateVersion));

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        snapshotRepository.Save(aggregate, aggregate.AggregateVersion);

        // Assert
        _snapshotsCacheMock.Verify(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true), Times.Never);
        _aggregateRepositoryMock.Verify(x => x.Save(aggregate, aggregate.AggregateVersion), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenVersionHasBeenProvided_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        int timeout = _faker.Random.Int(1, 1000);

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotsCacheMock.Setup(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true));
        _aggregateRepositoryMock.Setup(x => x.SaveAsync(aggregate, aggregate.AggregateVersion, default));

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        await snapshotRepository.SaveAsync(aggregate, aggregate.AggregateVersion, timeout);

        // Assert
        _snapshotsCacheMock.Verify(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true), Times.Once);
        _aggregateRepositoryMock.Verify(x => x.SaveAsync(aggregate, aggregate.AggregateVersion, default), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WithoutTimeout_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        int timeout = _faker.Random.Int(1, 1000);

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotsCacheMock.Setup(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true));
        _aggregateRepositoryMock.Setup(x => x.SaveAsync(aggregate, aggregate.AggregateVersion, default));

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        await snapshotRepository.SaveAsync(aggregate, aggregate.AggregateVersion);

        // Assert
        _snapshotsCacheMock.Verify(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true), Times.Never);
        _aggregateRepositoryMock.Verify(x => x.SaveAsync(aggregate, aggregate.AggregateVersion, default), Times.Once);
    }

    [Test]
    public void Box_CorrectAggregateProvided_ShouldBoxAggregate()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            AggregateState = aggregate.State
        };

        _snapshotMapperMock.Setup(x => x.Map(It.IsAny<Snapshot>())).Returns(snapshotDataModel);
        _snapshotStoreMock.Setup(x => x.Save(snapshotDataModel));

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        _snapshotStoreMock.Setup(x => x.Box(aggregate.AggregateIdentifier));
        _aggregateStoreMock.Setup(x => x.Box(aggregate.AggregateIdentifier));
        _snapshotsCacheMock.Setup(x => x.Remove(aggregate.AggregateIdentifier));

        // Act
        snapshotRepository.Box(aggregate);

        // Assert
        _snapshotStoreMock.Verify(x => x.Save(snapshotDataModel), Times.Once);
        _snapshotStoreMock.Verify(x => x.Box(aggregate.AggregateIdentifier), Times.Once);
        _aggregateStoreMock.Verify(x => x.Box(aggregate.AggregateIdentifier), Times.Once);
        _snapshotsCacheMock.Verify(x => x.Remove(aggregate.AggregateIdentifier), Times.Once);
    }

    [Test]
    public async Task BoxAsync_CorrectAggregateProvided_ShouldBoxAggregate()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            AggregateState = aggregate.State
        };

        _snapshotMapperMock.Setup(x => x.Map(It.IsAny<Snapshot>())).Returns(snapshotDataModel);
        _snapshotStoreMock.Setup(x => x.SaveAsync(snapshotDataModel, default));

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        _snapshotStoreMock.Setup(x => x.BoxAsync(aggregate.AggregateIdentifier, default));
        _aggregateStoreMock.Setup(x => x.BoxAsync(aggregate.AggregateIdentifier, default));
        _snapshotsCacheMock.Setup(x => x.Remove(aggregate.AggregateIdentifier));

        // Act
        await snapshotRepository.BoxAsync(aggregate);

        // Assert
        _snapshotStoreMock.Verify(x => x.SaveAsync(snapshotDataModel, default), Times.Once);
        _snapshotStoreMock.Verify(x => x.BoxAsync(aggregate.AggregateIdentifier, default), Times.Once);
        _aggregateStoreMock.Verify(x => x.BoxAsync(aggregate.AggregateIdentifier, default), Times.Once);
        _snapshotsCacheMock.Verify(x => x.Remove(aggregate.AggregateIdentifier), Times.Once);
    }

    [Test]
    public void Unbox_CorrectAggregateIdentifierProvided_ShouldUnboxAggregate()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            AggregateState = aggregate.State
        };

        Snapshot snapshot = new(snapshotDataModel.AggregateIdentifier, snapshotDataModel.AggregateVersion,
            snapshotDataModel.AggregateState);

        _snapshotStoreMock.Setup(x => x.Unbox(aggregateIdentifier)).Returns(snapshotDataModel);
        _snapshotMapperMock.Setup(x => x.Map(snapshotDataModel)).Returns(snapshot);

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        Post result = snapshotRepository.Unbox<Post>(aggregateIdentifier);

        // Assert
        _snapshotStoreMock.Verify(x => x.Unbox(aggregateIdentifier), Times.Once);
        _snapshotMapperMock.Verify(x => x.Map(snapshotDataModel), Times.Once);
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public async Task UnboxAsync_CorrectAggregateIdentifierProvided_ShouldUnboxAggregate()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        Post aggregate = GenericAggregateFactory<Post>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);

        List<IDomainEvent> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            AggregateState = aggregate.State
        };

        Snapshot snapshot = new(snapshotDataModel.AggregateIdentifier, snapshotDataModel.AggregateVersion,
            snapshotDataModel.AggregateState);

        _snapshotStoreMock.Setup(x => x.UnboxAsync(aggregateIdentifier, default)).ReturnsAsync(snapshotDataModel);
        _snapshotMapperMock.Setup(x => x.Map(snapshotDataModel)).Returns(snapshot);

        ISnapshotRepository snapshotRepository = new SnapshotRepository(_aggregateStoreMock.Object,
            _aggregateRepositoryMock.Object, _snapshotStoreMock.Object, _snapshotStrategyMock.Object,
            _serializerMock.Object, _snapshotsCacheMock.Object, _snapshotMapperMock.Object,
            _domainEventMapperMock.Object, _identityServiceMock.Object);

        // Act
        Post result = await snapshotRepository.UnboxAsync<Post>(aggregateIdentifier);

        // Assert
        _snapshotStoreMock.Verify(x => x.UnboxAsync(aggregateIdentifier, default), Times.Once);
        _snapshotMapperMock.Verify(x => x.Map(snapshotDataModel), Times.Once);
        result.Should().BeEquivalentTo(aggregate);
    }

    private List<IDomainEvent> GenerateDomainEventsCollection(Guid aggregateIdentifier)
    {
        List<IDomainEvent> domainEvents = new()
        {
            new CreateNewPostDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 1,
                Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
                Date.Create(_faker.Date.FutureOffset()), null),
            new ChangeDescriptionDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 2,
                Description.Create(_faker.Lorem.Sentences())),
            new ChangeTitleDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 3,
                Title.Default(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)))),
            new ChangeDescriptionDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 4,
                Description.Create(_faker.Lorem.Sentences()))
        };

        return domainEvents;
    }

    private List<DomainEventDataModel> GenerateDomainEventDataModelsCollection(Guid aggregateIdentifier)
    {
        IDomainEventMapper domainEventMapper = new DomainEventMapper();
        List<IDomainEvent> domainEvents = GenerateDomainEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> domainEventDms = domainEventMapper.Map(domainEvents).ToList();

        return domainEventDms;
    }
}