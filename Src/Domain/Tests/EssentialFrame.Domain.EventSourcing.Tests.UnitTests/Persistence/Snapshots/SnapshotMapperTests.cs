﻿using System;
using System.Collections.Generic;
using Bogus;
using EssentialFrame.Domain.EventSourcing.Core.Snapshots;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Mappers;
using EssentialFrame.Domain.EventSourcing.Persistence.Snapshots.Models;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.Identity.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.EventSourcing.Tests.UnitTests.Persistence.Snapshots;

[TestFixture]
public class SnapshotMapperTests
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
    }

    private readonly Faker _faker = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();

    [Test]
    public void Map_Always_ShouldMapSnapshotToSnapshotDataModel()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        string serializedState = _faker.Random.String(_faker.Random.Int(100, 300));
        Snapshot<PostIdentifier, Guid> snapshot = new(aggregateIdentifier, aggregateVersion, serializedState);
        SnapshotMapper<PostIdentifier, Guid> mapper = new();

        // Act
        SnapshotDataModel result = mapper.Map(snapshot);

        // Assert
        AssertSnapshotDataModel(result, aggregateIdentifier, aggregateVersion, serializedState);
    }

    [Test]
    public void Map_Always_ShouldMapSnapshotDataModelToSnapshot()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        string serializedState = _faker.Random.String(_faker.Random.Int(100, 300));
        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = aggregateVersion,
            AggregateState = serializedState
        };
        SnapshotMapper<PostIdentifier, Guid> mapper = new();

        // Act
        Snapshot<PostIdentifier, Guid> result = mapper.Map(snapshotDataModel);

        // Assert
        AssertSnapshot(result, aggregateIdentifier, aggregateVersion, serializedState);
    }

    [Test]
    public void Map_Always_ShouldMapCollectionOfSnapshots()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        string serializedState = _faker.Random.String(_faker.Random.Int(100, 300));
        Snapshot<PostIdentifier, Guid> snapshot = new(aggregateIdentifier, aggregateVersion, serializedState);
        Snapshot<PostIdentifier, Guid>[] snapshots = { snapshot };
        SnapshotMapper<PostIdentifier, Guid> snapshotMapper = new();

        // Act
        IReadOnlyCollection<SnapshotDataModel> result = snapshotMapper.Map(snapshots);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(snapshots.Length);

        foreach (SnapshotDataModel snapshotDataModel in result)
        {
            AssertSnapshotDataModel(snapshotDataModel, aggregateIdentifier, aggregateVersion, serializedState);
        }
    }

    [Test]
    public void Map_Always_ShouldMapCollectionOfSnapshotDataModels()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        string serializedState = _faker.Random.String(_faker.Random.Int(100, 300));
        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = aggregateVersion,
            AggregateState = serializedState
        };
        SnapshotDataModel[] snapshotDataModels = { snapshotDataModel };
        SnapshotMapper<PostIdentifier, Guid> mapper = new();

        // Act
        IReadOnlyCollection<Snapshot<PostIdentifier, Guid>> result = mapper.Map(snapshotDataModels);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(snapshotDataModels.Length);

        foreach (Snapshot<PostIdentifier, Guid> snapshot in result)
        {
            AssertSnapshot(snapshot, aggregateIdentifier, aggregateVersion, serializedState);
        }
    }

    private void AssertSnapshot(Snapshot<PostIdentifier, Guid> snapshot, PostIdentifier aggregateIdentifier,
        int aggregateVersion, string serializedState)
    {
        snapshot.AggregateIdentifier.Value.Should().Be(aggregateIdentifier.Value);
        snapshot.AggregateVersion.Should().Be(aggregateVersion);
        snapshot.AggregateState.Should().Be(serializedState);
    }

    private void AssertSnapshotDataModel(SnapshotDataModel snapshotDataModel, PostIdentifier aggregateIdentifier,
        int aggregateVersion, string serializedState)
    {
        snapshotDataModel.AggregateIdentifier.Should().Be(aggregateIdentifier);
        snapshotDataModel.AggregateVersion.Should().Be(aggregateVersion);
        snapshotDataModel.AggregateState.Should().Be(serializedState);
    }
}