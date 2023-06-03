using System;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Snapshots;
using EssentialFrame.Domain.Events.Snapshots.Interfaces;
using EssentialFrame.Domain.Snapshots;
using EssentialFrame.Identity;
using EssentialFrame.TestData.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.UnitTests.Snapshots;

[TestFixture]
public class DefaultSnapshotStoreTests
{
    private readonly Faker _faker = new();
    private Mock<ICache<Guid, Snapshot>> _snapshotCacheMock;
    private Mock<IIdentityService> _identityServiceMock;
    private Mock<ISnapshotOfflineStorage> _snapshotOfflineStorageMock;

    [SetUp]
    public void SetUp()
    {
        _snapshotCacheMock = new Mock<ICache<Guid, Snapshot>>();
        _identityServiceMock = new Mock<IIdentityService>();
        _snapshotOfflineStorageMock = new Mock<ISnapshotOfflineStorage>();

        _identityServiceMock.Setup(x => x.GetCurrent()).Returns(new TestIdentity());
    }

    [TearDown]
    public void TearDown()
    {
        _snapshotCacheMock.Reset();
        _identityServiceMock.Reset();
        _snapshotOfflineStorageMock.Reset();
    }

    [Test]
    public void Get_WhenAggregateIdentifierIsProvided_ShouldReturnSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        Snapshot snapshot = new Snapshot(aggregateIdentifier, 1, _identityServiceMock.Object.GetCurrent());
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshot);
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Snapshot result = snapshotStore.Get(aggregateIdentifier);

        // Assert
        result.Should().Be(snapshot);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIdentifierIsProvided_ShouldReturnSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        Snapshot snapshot = new Snapshot(aggregateIdentifier, 1, _identityServiceMock.Object.GetCurrent());
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshot);
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Snapshot result = await snapshotStore.GetAsync(aggregateIdentifier);

        // Assert
        result.Should().Be(snapshot);
    }

    [Test]
    public void Get_WhenAggregateIdentifierIsProvidedAndSnapshotDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((Snapshot)null);
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Snapshot result = snapshotStore.Get(aggregateIdentifier);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetAsync_WhenAggregateIdentifierIsProvidedAndSnapshotDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((Snapshot)null);
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Snapshot result = await snapshotStore.GetAsync(aggregateIdentifier);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void Save_WhenSnapshotIsProvided_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        Snapshot snapshot = new Snapshot(aggregateIdentifier, 1, _identityServiceMock.Object.GetCurrent());
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        snapshotStore.Save(snapshot);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshot), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenSnapshotIsProvided_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        Snapshot snapshot = new Snapshot(aggregateIdentifier, 1, _identityServiceMock.Object.GetCurrent());
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        await snapshotStore.SaveAsync(snapshot);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshot), Times.Once);
    }

    [Test]
    public void Save_WhenSnapshotIsProvidedAndSnapshotAlreadyExists_ShouldUpdateSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        Snapshot snapshot = new Snapshot(aggregateIdentifier, 1, _identityServiceMock.Object.GetCurrent());
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshot);
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        snapshotStore.Save(snapshot);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshot), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenSnapshotIsProvidedAndSnapshotAlreadyExists_ShouldUpdateSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        Snapshot snapshot = new Snapshot(aggregateIdentifier, 1, _identityServiceMock.Object.GetCurrent());
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshot);
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        await snapshotStore.SaveAsync(snapshot);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshot), Times.Once);
    }

    [Test]
    public void Save_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        Snapshot snapshot = new Snapshot(aggregateIdentifier, 1, _identityServiceMock.Object.GetCurrent());
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((Snapshot)null);
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        snapshotStore.Save(snapshot);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshot), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldSaveSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        Snapshot snapshot = new Snapshot(aggregateIdentifier, 1, _identityServiceMock.Object.GetCurrent());
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((Snapshot)null);
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        await snapshotStore.SaveAsync(snapshot);

        // Assert
        _snapshotCacheMock.Verify(x => x.Add(aggregateIdentifier, snapshot), Times.Once);
    }

    [Test]
    public void Box_WhenSnapshotIsProvided_ShouldBoxSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        Snapshot snapshot = new Snapshot(aggregateIdentifier, 1, _identityServiceMock.Object.GetCurrent());
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshot);
        _snapshotOfflineStorageMock.Setup(x => x.Save(snapshot));
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        snapshotStore.Box(aggregateIdentifier);

        // Assert
        _snapshotCacheMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
        _snapshotOfflineStorageMock.Verify(x => x.Save(snapshot), Times.Once);
    }

    [Test]
    public async Task BoxAsync_WhenSnapshotIsProvided_ShouldBoxSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        Snapshot snapshot = new Snapshot(aggregateIdentifier, 1, _identityServiceMock.Object.GetCurrent());
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(snapshot);
        _snapshotOfflineStorageMock.Setup(x => x.SaveAsync(snapshot, default));
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        await snapshotStore.BoxAsync(aggregateIdentifier);

        // Assert
        _snapshotCacheMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
        _snapshotOfflineStorageMock.Verify(x => x.SaveAsync(snapshot, default), Times.Once);
    }

    [Test]
    public void Box_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldThrowException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((Snapshot)null);
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

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
        _snapshotCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns((Snapshot)null);
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

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
        Snapshot snapshot = new Snapshot(aggregateIdentifier, 1, _identityServiceMock.Object.GetCurrent());
        _snapshotOfflineStorageMock.Setup(x => x.Restore(aggregateIdentifier)).Returns(snapshot);
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Snapshot result = snapshotStore.Unbox(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(snapshot);
        _snapshotOfflineStorageMock.Verify(x => x.Restore(aggregateIdentifier), Times.Once);
    }

    [Test]
    public async Task UnboxAsync_WhenSnapshotIsProvided_ShouldUnboxSnapshot()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        Snapshot snapshot = new Snapshot(aggregateIdentifier, 1, _identityServiceMock.Object.GetCurrent());
        _snapshotOfflineStorageMock.Setup(x => x.RestoreAsync(aggregateIdentifier, default)).ReturnsAsync(snapshot);
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Snapshot result = await snapshotStore.UnboxAsync(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(snapshot);
        _snapshotOfflineStorageMock.Verify(x => x.RestoreAsync(aggregateIdentifier, default), Times.Once);
    }

    [Test]
    public void Unbox_WhenSnapshotIsProvidedAndSnapshotDoesNotExist_ShouldThrowException()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _snapshotOfflineStorageMock.Setup(x => x.Restore(aggregateIdentifier))
            .Throws(SnapshotUnboxingFailedException.SnapshotNotFound(aggregateIdentifier));
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

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
        DefaultSnapshotStore snapshotStore =
            new DefaultSnapshotStore(_snapshotCacheMock.Object, _snapshotOfflineStorageMock.Object);

        // Act
        Func<Task> act = async () => await snapshotStore.UnboxAsync(aggregateIdentifier);

        // Assert
        await act.Should().ThrowExactlyAsync<SnapshotUnboxingFailedException>().WithMessage(
            $"Unable to unbox snapshot for aggregate with id: ({aggregateIdentifier}), because snapshot hasn't been found. See inner exception for more details.");
    }
}