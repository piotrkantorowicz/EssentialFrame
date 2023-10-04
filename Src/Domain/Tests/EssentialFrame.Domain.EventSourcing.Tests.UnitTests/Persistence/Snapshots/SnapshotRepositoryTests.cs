﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Core.Events.Interfaces;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Factories;
using EssentialFrame.Domain.EventSourcing.Core.Snapshots;
using EssentialFrame.Domain.EventSourcing.Core.Snapshots.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Services.Interfaces;
using EssentialFrame.Domain.Persistence.Mappers;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents.Events;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Identity.Interfaces;
using EssentialFrame.Serialization.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.EventSourcing.Tests.UnitTests.Persistence.Snapshots;

[TestFixture]
public class SnapshotRepositoryTests
{
    private readonly Mock<IEventSourcingAggregateRepository<Post, PostIdentifier, Guid>> _aggregateRepositoryMock =
        new();
    private readonly Mock<IEventSourcingAggregateStore> _aggregateStoreMock = new();
    private readonly Mock<ICache<PostIdentifier, Post>> _snapshotsCacheMock = new();
    private readonly Mock<IDomainEventMapper<PostIdentifier, Guid>> _domainEventMapperMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<ISerializer> _serializerMock = new();
    private readonly Mock<ISnapshotMapper<PostIdentifier, Guid>> _snapshotMapperMock = new();
    private readonly Mock<ISnapshotStore> _snapshotStoreMock = new();
    private readonly Mock<ISnapshotStrategy<Post, PostIdentifier, Guid>> _snapshotStrategyMock = new();
    private readonly Faker _faker = new();

    [SetUp]
    public void SetUp()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new AppIdentityContext());
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
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Number();

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        _snapshotsCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(aggregate);

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        Post result = await snapshotRepository.GetAsync(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIsInCache_ShouldReturnAggregateFromSnapshotCache()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Number();

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        _snapshotsCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(aggregate);

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        Post result = await snapshotRepository.GetAsync(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public void Get_WhenAggregateIsNotInCacheButExistsInDatabase_ShouldReturnAggregateFromSnapshotDatabase()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> eventDataModels = GenerateDomainEventDataModelsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = events.Count,
            AggregateState = aggregate.State
        };

        Snapshot<PostIdentifier, Guid> snapshot = new(
            TypedIdentifierBase<Guid>.New<PostIdentifier>(snapshotDataModel.AggregateIdentifier),
            snapshotDataModel.AggregateVersion, snapshotDataModel.AggregateState);

        _snapshotStoreMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        _snapshotMapperMock.Setup(x => x.Map(snapshotDataModel)).Returns(snapshot);
        _aggregateStoreMock.Setup(x => x.Get(aggregateIdentifier, snapshot.AggregateVersion))
            .Returns(eventDataModels);
        _domainEventMapperMock.Setup(x => x.Map(eventDataModels)).Returns(events);

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        Post result = snapshotRepository.Get(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIsNotInCacheButExistsInDatabase_ShouldReturnAggregateFromSnapshotDatabase()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> eventDataModels = GenerateDomainEventDataModelsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = events.Count,
            AggregateState = aggregate.State
        };

        Snapshot<PostIdentifier, Guid> snapshot = new(
            TypedIdentifierBase<Guid>.New<PostIdentifier>(snapshotDataModel.AggregateIdentifier),
            snapshotDataModel.AggregateVersion, snapshotDataModel.AggregateState);

        _snapshotStoreMock.Setup(x => x.GetAsync(aggregateIdentifier, default)).ReturnsAsync(snapshotDataModel);
        _snapshotMapperMock.Setup(x => x.Map(snapshotDataModel)).Returns(snapshot);

        _aggregateStoreMock.Setup(x => x.GetAsync(aggregateIdentifier, snapshot.AggregateVersion, default))
            .ReturnsAsync(eventDataModels);

        _domainEventMapperMock.Setup(x => x.Map(eventDataModels)).Returns(events);

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        Post result = await snapshotRepository.GetAsync(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public void Get_WhenAggregateIsNotInCacheAndSnapshotDatabase_ShouldReturnAggregateFromAggregateDatabase()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotStoreMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);

        _aggregateRepositoryMock.Setup(x => x.Get(aggregateIdentifier)).Returns(aggregate);

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        Post result = snapshotRepository.Get(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIsNotInCacheAndSnapshotDatabase_ShouldReturnAggregateFromAggregateDatabase()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotStoreMock.Setup(x => x.GetAsync(aggregateIdentifier, default))
            .ReturnsAsync((SnapshotDataModel)null);

        _aggregateRepositoryMock.Setup(x => x.GetAsync(aggregateIdentifier, default)).ReturnsAsync(aggregate);

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        Post result = await snapshotRepository.GetAsync(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public void Save_WhenVersionHasNotBeenProvided_ShouldSaveSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;
        int timeout = _faker.Random.Int(1, 1000);

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotsCacheMock.Setup(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true));
        _aggregateRepositoryMock.Setup(x => x.Save(aggregate, null));

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        snapshotRepository.Save(aggregate, timeout: timeout);

        // Assert
        _snapshotsCacheMock.Verify(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true),
            Times.Once);
        _aggregateRepositoryMock.Verify(x => x.Save(aggregate, null), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenVersionHasNotBeenProvided_ShouldSaveSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;
        int timeout = _faker.Random.Int(1, 1000);

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotsCacheMock.Setup(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true));
        _aggregateRepositoryMock.Setup(x => x.SaveAsync(aggregate, null, default));

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        await snapshotRepository.SaveAsync(aggregate, timeout: timeout);

        // Assert
        _snapshotsCacheMock.Verify(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true),
            Times.Once);
        _aggregateRepositoryMock.Verify(x => x.SaveAsync(aggregate, null, default), Times.Once);
    }

    [Test]
    public void Save_WhenVersionHasBeenProvided_ShouldSaveSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;
        int timeout = _faker.Random.Int(1, 1000);

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotsCacheMock.Setup(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true));
        _aggregateRepositoryMock.Setup(x => x.Save(aggregate, aggregate.AggregateVersion));

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        snapshotRepository.Save(aggregate, aggregate.AggregateVersion, timeout);

        // Assert
        _snapshotsCacheMock.Verify(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true),
            Times.Once);
        _aggregateRepositoryMock.Verify(x => x.Save(aggregate, aggregate.AggregateVersion), Times.Once);
    }


    [Test]
    public void Save_WithoutTimeout_ShouldSaveSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;
        int timeout = _faker.Random.Int(1, 1000);

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotsCacheMock.Setup(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true));
        _aggregateRepositoryMock.Setup(x => x.Save(aggregate, aggregate.AggregateVersion));

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        snapshotRepository.Save(aggregate, aggregate.AggregateVersion);

        // Assert
        _snapshotsCacheMock.Verify(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true),
            Times.Never);
        _aggregateRepositoryMock.Verify(x => x.Save(aggregate, aggregate.AggregateVersion), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenVersionHasBeenProvided_ShouldSaveSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;
        int timeout = _faker.Random.Int(1, 1000);

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotsCacheMock.Setup(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true));
        _aggregateRepositoryMock.Setup(x => x.SaveAsync(aggregate, aggregate.AggregateVersion, default));

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        await snapshotRepository.SaveAsync(aggregate, aggregate.AggregateVersion, timeout);

        // Assert
        _snapshotsCacheMock.Verify(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true),
            Times.Once);
        _aggregateRepositoryMock.Verify(x => x.SaveAsync(aggregate, aggregate.AggregateVersion, default), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WithoutTimeout_ShouldSaveSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;
        int timeout = _faker.Random.Int(1, 1000);

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _snapshotsCacheMock.Setup(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true));
        _aggregateRepositoryMock.Setup(x => x.SaveAsync(aggregate, aggregate.AggregateVersion, default));

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        await snapshotRepository.SaveAsync(aggregate, aggregate.AggregateVersion);

        // Assert
        _snapshotsCacheMock.Verify(x => x.Add(aggregate.AggregateIdentifier, aggregate, timeout, true),
            Times.Never);
        _aggregateRepositoryMock.Verify(x => x.SaveAsync(aggregate, aggregate.AggregateVersion, default), Times.Once);
    }

    [Test]
    public void Box_CorrectAggregateProvided_ShouldBoxAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            AggregateState = aggregate.State
        };

        _snapshotMapperMock.Setup(x => x.Map(It.IsAny<Snapshot<PostIdentifier, Guid>>())).Returns(snapshotDataModel);
        _snapshotStoreMock.Setup(x => x.Save(snapshotDataModel));

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

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
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            AggregateState = aggregate.State
        };

        _snapshotMapperMock.Setup(x => x.Map(It.IsAny<Snapshot<PostIdentifier, Guid>>())).Returns(snapshotDataModel);
        _snapshotStoreMock.Setup(x => x.SaveAsync(snapshotDataModel, default));

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

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
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            AggregateState = aggregate.State
        };

        Snapshot<PostIdentifier, Guid> snapshot = new(
            TypedIdentifierBase<Guid>.New<PostIdentifier>(snapshotDataModel.AggregateIdentifier),
            snapshotDataModel.AggregateVersion, snapshotDataModel.AggregateState);

        _snapshotStoreMock.Setup(x => x.Unbox(aggregateIdentifier)).Returns(snapshotDataModel);
        _snapshotMapperMock.Setup(x => x.Map(snapshotDataModel)).Returns(snapshot);

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        Post result = snapshotRepository.Unbox(aggregateIdentifier);

        // Assert
        _snapshotStoreMock.Verify(x => x.Unbox(aggregateIdentifier), Times.Once);
        _snapshotMapperMock.Verify(x => x.Map(snapshotDataModel), Times.Once);
        result.Should().BeEquivalentTo(aggregate);
    }

    [Test]
    public async Task UnboxAsync_CorrectAggregateIdentifierProvided_ShouldUnboxAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate = EventSourcingGenericAggregateFactory<Post, PostIdentifier, Guid>.CreateAggregate(
            aggregateIdentifier,
                aggregateVersion);

        List<IDomainEvent<PostIdentifier, Guid>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier,
            AggregateVersion = aggregate.AggregateVersion,
            AggregateState = aggregate.State
        };

        Snapshot<PostIdentifier, Guid> snapshot = new(
            TypedIdentifierBase<Guid>.New<PostIdentifier>(snapshotDataModel.AggregateIdentifier),
            snapshotDataModel.AggregateVersion, snapshotDataModel.AggregateState);

        _snapshotStoreMock.Setup(x => x.UnboxAsync(aggregateIdentifier, default)).ReturnsAsync(snapshotDataModel);
        _snapshotMapperMock.Setup(x => x.Map(snapshotDataModel)).Returns(snapshot);

        ISnapshotRepository<Post, PostIdentifier, Guid> snapshotRepository =
            new SnapshotRepository<Post, PostIdentifier, Guid>(
            _aggregateStoreMock.Object, _aggregateRepositoryMock.Object, _snapshotStoreMock.Object,
            _snapshotStrategyMock.Object, _serializerMock.Object, _snapshotsCacheMock.Object,
            _snapshotMapperMock.Object, _domainEventMapperMock.Object);

        // Act
        Post result = await snapshotRepository.UnboxAsync(aggregateIdentifier);

        // Assert
        _snapshotStoreMock.Verify(x => x.UnboxAsync(aggregateIdentifier, default), Times.Once);
        _snapshotMapperMock.Verify(x => x.Map(snapshotDataModel), Times.Once);
        result.Should().BeEquivalentTo(aggregate);
    }

    private List<IDomainEvent<PostIdentifier, Guid>> GenerateDomainEventsCollection(PostIdentifier aggregateIdentifier)
    {
        List<IDomainEvent<PostIdentifier, Guid>> domainEvents = new()
        {
            new NewPostCreatedDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 1,
                Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
                Date.Create(_faker.Date.FutureOffset()), null),
            new DescriptionChangedDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 2,
                Description.Create(_faker.Lorem.Sentences())),
            new TitleChangedDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 3,
                Title.Default(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)))),
            new DescriptionChangedDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 4,
                Description.Create(_faker.Lorem.Sentences()))
        };

        return domainEvents;
    }

    private List<DomainEventDataModel> GenerateDomainEventDataModelsCollection(PostIdentifier aggregateIdentifier)
    {
        IDomainEventMapper<PostIdentifier, Guid> domainEventMapper = new DomainEventMapper<PostIdentifier, Guid>();
        List<IDomainEvent<PostIdentifier, Guid>> domainEvents = GenerateDomainEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> domainEventDms = domainEventMapper.Map(domainEvents).ToList();

        return domainEventDms;
    }
}