using System;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Models;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Services;
using EssentialFrame.Domain.Events.Persistence.Snapshots.Services.Interfaces;
using EssentialFrame.ExampleApp.Identity;
using EssentialFrame.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Persistence.Snapshots;

[TestFixture]
public class DefaultSnapshotStoreTests
{
    [SetUp]
    public void SetUp()
    {
        _snapshotCacheMock = new Mock<ICache<Guid, SnapshotDataModel>>();
        _identityServiceMock = new Mock<IIdentityService>();
        _snapshotOfflineStorageMock = new Mock<ISnapshotOfflineStorage>();

        _identityServiceMock.Setup(x => x.GetCurrent()).Returns(new IdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _snapshotCacheMock.Reset();
        _identityServiceMock.Reset();
        _snapshotOfflineStorageMock.Reset();
    }

    private readonly Faker _faker = new();
    private Mock<ICache<Guid, SnapshotDataModel>> _snapshotCacheMock;
    private Mock<IIdentityService> _identityServiceMock;
    private Mock<ISnapshotOfflineStorage> _snapshotOfflineStorageMock;

    [Test]
    public void Get_WhenAggregateIdentifierIsProvided_ShouldReturnSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        SnapshotDataModel result = snapshotStore.Get(aggregateIdentifier);

        // Assert
        result.Should().Be(snapshotDataModel);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIdentifierIsProvided_ShouldReturnSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        SnapshotDataModel result = await snapshotStore.GetAsync(aggregateIdentifier);

        // Assert
        result.Should().Be(snapshotDataModel);
    }

    [Test]
    public void Get_WhenAggregateIdentifierIsProvidedAndSnapshotDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        SnapshotDataModel result = snapshotStore.Get(aggregateIdentifier);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetAsync_WhenAggregateIdentifierIsProvidedAndSnapshotDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        SnapshotDataModel result = await snapshotStore.GetAsync(aggregateIdentifier);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void Save_WhenSnapshotIsProvided_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        snapshotStore.Save(snapshotDataModel);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshotDataModel), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenSnapshotIsProvided_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        await snapshotStore.SaveAsync(snapshotDataModel);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshotDataModel), Times.Once);
    }

    [Test]
    public void Save_WhenSnapshotIsProvidedAndSnapshotAlreadyExists_ShouldUpdateSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        snapshotStore.Save(snapshotDataModel);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshotDataModel), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenSnapshotIsProvidedAndSnapshotAlreadyExists_ShouldUpdateSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        await snapshotStore.SaveAsync(snapshotDataModel);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshotDataModel), Times.Once);
    }

    [Test]
    public void Save_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        snapshotStore.Save(snapshotDataModel);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshotDataModel), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        await snapshotStore.SaveAsync(snapshotDataModel);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshotDataModel), Times.Once);
    }

    [Test]
    public void Box_WhenSnapshotIsProvided_ShouldBoxSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        _snapshotOfflineStorageMock.Setup(x => x.Save(snapshotDataModel));
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        snapshotStore.Box(aggregateIdentifier);

        // Assert
        _snapshotCacheMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
        _snapshotOfflineStorageMock.Verify(x => x.Save(snapshotDataModel), Times.Once);
    }

    [Test]
    public async Task BoxAsync_WhenSnapshotIsProvided_ShouldBoxSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshotDataModel);
        _snapshotOfflineStorageMock.Setup(x => x.SaveAsync(snapshotDataModel, default));
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        await snapshotStore.BoxAsync(aggregateIdentifier);

        // Assert
        _snapshotCacheMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
        _snapshotOfflineStorageMock.Verify(x => x.SaveAsync(snapshotDataModel, default), Times.Once);
    }

    [Test]
    public void Box_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldThrowException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Action act = () => snapshotStore.Box(aggregateIdentifier);

        // Assert
        act.Should().ThrowExactly<SnapshotBoxingFailedException>().WithMessage(
            $"Unable to box snapshot for aggregate with id: ({aggregateIdentifier}), because snapshot hasn't been found. See inner exception for more details.");
    }

    [Test]
    public async Task BoxAsync_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldThrowException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((SnapshotDataModel)null);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Func<Task> act = async () => await snapshotStore.BoxAsync(aggregateIdentifier);

        // Assert
        await act.Should().ThrowExactlyAsync<SnapshotBoxingFailedException>().WithMessage(
            $"Unable to box snapshot for aggregate with id: ({aggregateIdentifier}), because snapshot hasn't been found. See inner exception for more details.");
    }

    [Test]
    public void Unbox_WhenSnapshotIsProvided_ShouldUnboxSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotOfflineStorageMock.Setup(x => x.Restore(aggregateIdentifier)).Returns(snapshotDataModel);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        SnapshotDataModel result = snapshotStore.Unbox(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(snapshotDataModel);
        _snapshotOfflineStorageMock.Verify(x => x.Restore(aggregateIdentifier), Times.Once);
    }

    [Test]
    public async Task UnboxAsync_WhenSnapshotIsProvided_ShouldUnboxSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        SnapshotDataModel snapshotDataModel = GetSnapshotDataModel(aggregateIdentifier);
        _snapshotOfflineStorageMock.Setup(x => x.RestoreAsync(aggregateIdentifier, default))
            .ReturnsAsync(snapshotDataModel);
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        SnapshotDataModel result = await snapshotStore.UnboxAsync(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(snapshotDataModel);
        _snapshotOfflineStorageMock.Verify(x => x.RestoreAsync(aggregateIdentifier, default), Times.Once);
    }

    [Test]
    public void Unbox_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldThrowException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _snapshotOfflineStorageMock.Setup(x => x.Restore(aggregateIdentifier))
            .Throws(SnapshotUnboxingFailedException.SnapshotNotFound(aggregateIdentifier));
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Action act = () => snapshotStore.Unbox(aggregateIdentifier);

        // Assert
        act.Should().ThrowExactly<SnapshotUnboxingFailedException>().WithMessage(
            $"Unable to unbox snapshot for aggregate with id: ({aggregateIdentifier}), because snapshot hasn't been found. See inner exception for more details.");
    }

    [Test]
    public async Task UnboxAsync_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldThrowException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _snapshotOfflineStorageMock.Setup(x => x.RestoreAsync(aggregateIdentifier, default))
            .Throws(SnapshotUnboxingFailedException.SnapshotNotFound(aggregateIdentifier));
        DefaultSnapshotStore snapshotStore = new(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Func<Task> act = async () => await snapshotStore.UnboxAsync(aggregateIdentifier);

        // Assert
        await act.Should().ThrowExactlyAsync<SnapshotUnboxingFailedException>().WithMessage(
            $"Unable to unbox snapshot for aggregate with id: ({aggregateIdentifier}), because snapshot hasn't been found. See inner exception for more details.");
    }

    private SnapshotDataModel GetSnapshotDataModel(Guid aggregateIdentifier)
    {
        string serializedAggregateState = _faker.Random.String(_faker.Random.Int(1, 250));
        SnapshotDataModel snapshotDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier,
            AggregateVersion = 1,
            AggregateState = serializedAggregateState
        };

        return snapshotDataModel;
    }
}