using System;
using System.IO;
using System.IO.Abstractions;
using System.Security;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Snapshots;
using EssentialFrame.Domain.Events.Snapshots.Interfaces;
using EssentialFrame.Domain.Snapshots;
using EssentialFrame.Files;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.TestData.Identity;
using EssentialFrame.TestData.Utils;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.UnitTests.Snapshots;

[TestFixture]
public class SnapshotOfflineStorageTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IFileStorage> _fileStorageMock = new();
    private readonly Mock<ISerializer> _serializerMock = new();
    private readonly Mock<IFileSystem> _fileSystemMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();

    private ILogger<SnapshotOfflineStorage> _logger;
    private Snapshot _snapshot;

    private static object[] _possibleExceptions =
    {
        new object[] { typeof(OutOfMemoryException) }, new object[] { typeof(ArgumentException) },
        new object[] { typeof(ArgumentNullException) }, new object[] { typeof(PathTooLongException) },
        new object[] { typeof(DirectoryNotFoundException) }, new object[] { typeof(IOException) },
        new object[] { typeof(UnauthorizedAccessException) }, new object[] { typeof(NotSupportedException) },
        new object[] { typeof(PathTooLongException) }, new object[] { typeof(UnauthorizedAccessException) },
        new object[] { typeof(SecurityException) }
    };

    [SetUp]
    public void SetUp()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new TestIdentity());
        _logger = NullLoggerFactory.Instance.CreateLogger<SnapshotOfflineStorage>();
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;
        string aggregateState = _faker.Lorem.Sentences();

        _snapshot = new Snapshot(aggregateIdentifier, aggregateVersion, aggregateState);
    }

    [TearDown]
    public void TearDown()
    {
        _fileStorageMock.Reset();
        _serializerMock.Reset();
        _fileSystemMock.Reset();
        _identityServiceMock.Reset();
        _logger = null;
    }

    [Test]
    public void Save_UsingDefaultDirectory_ShouldSaveFile()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        // Act
        snapShotOfflineStorage.Save(_snapshot);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()),
            Times.Once);

        _fileStorageMock.Verify(
            x => x.Create(directoryPath, It.IsAny<string>(), _snapshot.AggregateState.ToString(), null), Times.Once);
    }

    [Test]
    public async Task SaveAsync_UsingDefaultDirectory_ShouldSaveFile()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        // Act
        await snapShotOfflineStorage.SaveAsync(_snapshot);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()),
            Times.Once);

        _fileStorageMock.Verify(
            x => x.CreateAsync(directoryPath, It.IsAny<string>(), _snapshot.AggregateState.ToString(), null, default),
            Times.Once);
    }

    [Test]
    public void Save_UsingCustomDirectory_ShouldSaveFile()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, offlineDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        // Act
        snapShotOfflineStorage.Save(_snapshot);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()),
            Times.Once);

        _fileStorageMock.Verify(
            x => x.Create(directoryPath, It.IsAny<string>(), _snapshot.AggregateState.ToString(), null), Times.Once);
    }

    [Test]
    public async Task SaveAsync_UsingCustomDirectory_ShouldSaveFile()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, offlineDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        // Act
        await snapShotOfflineStorage.SaveAsync(_snapshot);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()),
            Times.Once);

        _fileStorageMock.Verify(
            x => x.CreateAsync(directoryPath, It.IsAny<string>(), _snapshot.AggregateState.ToString(), null, default),
            Times.Once);
    }

    [Test]
    public void Save_WhenStateIsAnObject_ShouldSaveFile()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();
        string[] state = _faker.Random.WordsArray(10);
        Snapshot snapshot = new Snapshot(_snapshot.AggregateIdentifier, _snapshot.AggregateVersion, state);
        string serializedState = _faker.Lorem.Sentences();

        _serializerMock.Setup(x => x.Serialize(snapshot.AggregateState)).Returns(serializedState);

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        // Act
        snapShotOfflineStorage.Save(snapshot);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), snapshot.AggregateIdentifier.ToString()),
            Times.Once);

        _serializerMock.Verify(x => x.Serialize(snapshot.AggregateState), Times.Once);

        _fileStorageMock.Verify(x => x.Create(directoryPath, It.IsAny<string>(), serializedState, null), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenStateIsAnObject_ShouldSaveFile()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();
        string[] state = _faker.Random.WordsArray(10);
        Snapshot snapshot = new Snapshot(_snapshot.AggregateIdentifier, _snapshot.AggregateVersion, state);
        string serializedState = _faker.Lorem.Sentences();

        _serializerMock.Setup(x => x.Serialize(snapshot.AggregateState)).Returns(serializedState);

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        // Act
        await snapShotOfflineStorage.SaveAsync(snapshot);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), snapshot.AggregateIdentifier.ToString()),
            Times.Once);

        _serializerMock.Verify(x => x.Serialize(snapshot.AggregateState), Times.Once);

        _fileStorageMock.Verify(x => x.CreateAsync(directoryPath, It.IsAny<string>(), serializedState, null, default),
            Times.Once);
    }

    [Test]
    [TestCaseSource(nameof(_possibleExceptions))]
    public void Save_WhenExceptionOccurs_ShouldCatchAndThrowAggregateBoxingFailedException(Type exception)
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock.Setup(x =>
                x.Create(directoryPath, It.IsAny<string>(), _snapshot.AggregateState.ToString(), null))
            .Throws(Activator.CreateInstance(exception) as Exception);

        // Act
        Action action = () => snapShotOfflineStorage.Save(_snapshot);

        // Assert
        action.Should().ThrowExactly<SnapshotBoxingFailedException>().WithMessage(
                $"Unexpected error while boxing snapshot for aggregate with id: ({_snapshot.AggregateIdentifier}). See inner exception for more details.")
            .WithInnerExceptionExactly(exception);
    }

    [Test]
    [TestCaseSource(nameof(_possibleExceptions))]
    public async Task SaveAsync_WhenExceptionOccurs_ShouldCatchAndThrowAggregateBoxingFailedException(Type exception)
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock.Setup(x =>
                x.CreateAsync(directoryPath, It.IsAny<string>(), _snapshot.AggregateState.ToString(), null, default))
            .Throws(Activator.CreateInstance(exception) as Exception);

        // Act
        Func<Task> action = async () => await snapShotOfflineStorage.SaveAsync(_snapshot);

        // Assert
        await action.Should().ThrowExactlyAsync<SnapshotBoxingFailedException>().WithMessage(
                $"Unexpected error while boxing snapshot for aggregate with id: ({_snapshot.AggregateIdentifier}). See inner exception for more details.")
            .WithInnerExceptionExactly(exception);
    }

    [Test]
    public void Restore_UsingDefaultDirectory_ShouldRestoreAggregate()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _fileStorageMock.Setup(x => x.Read(directoryPath, It.IsAny<string>(), null)).Returns(serializedState);

        // Act
        Snapshot snapshot = snapShotOfflineStorage.Restore(_snapshot.AggregateIdentifier);

        // Assert
        snapshot.AggregateIdentifier.Should().Be(_snapshot.AggregateIdentifier);
        snapshot.AggregateVersion.Should().Be(Defaults.DefaultAggregateVersion);
        snapshot.AggregateState.Should().Be(serializedState);
    }

    [Test]
    public async Task RestoreAsync_UsingDefaultDirectory_ShouldRestoreAggregate()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _fileStorageMock.Setup(x => x.ReadAsync(directoryPath, It.IsAny<string>(), null, default))
            .ReturnsAsync(serializedState);

        // Act
        Snapshot snapshot = await snapShotOfflineStorage.RestoreAsync(_snapshot.AggregateIdentifier);

        // Assert
        snapshot.AggregateIdentifier.Should().Be(_snapshot.AggregateIdentifier);
        snapshot.AggregateVersion.Should().Be(Defaults.DefaultAggregateVersion);
        snapshot.AggregateState.Should().Be(serializedState);
    }

    [Test]
    public void Restore_UsingCustomDirectory_ShouldRestoreAggregate()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, offlineDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _fileStorageMock.Setup(x => x.Read(directoryPath, It.IsAny<string>(), null)).Returns(serializedState);

        // Act
        Snapshot snapshot = snapShotOfflineStorage.Restore(_snapshot.AggregateIdentifier);

        // Assert
        snapshot.AggregateIdentifier.Should().Be(_snapshot.AggregateIdentifier);
        snapshot.AggregateVersion.Should().Be(Defaults.DefaultAggregateVersion);
        snapshot.AggregateState.Should().Be(serializedState);
    }

    [Test]
    public void RestoreAsync_UsingCustomDirectory_ShouldRestoreAggregate()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, offlineDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _fileStorageMock.Setup(x => x.ReadAsync(directoryPath, It.IsAny<string>(), null, default))
            .ReturnsAsync(serializedState);

        // Act
        Snapshot snapshot = snapShotOfflineStorage.RestoreAsync(_snapshot.AggregateIdentifier).GetAwaiter().GetResult();

        // Assert
        snapshot.AggregateIdentifier.Should().Be(_snapshot.AggregateIdentifier);
        snapshot.AggregateVersion.Should().Be(Defaults.DefaultAggregateVersion);
        snapshot.AggregateState.Should().Be(serializedState);
    }

    [Test]
    public void Restore_WhenAggregateNotFound_ShouldThrowFileNotFoundException()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();
        string filePath = _faker.System.FilePath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileSystemMock.Setup(x => x.Path.Combine(directoryPath, filePath)).Returns(directoryPath + filePath);

        FileNotFoundException fileNotFoundException =
            new($"File not found: {_fileSystemMock.Object.Path.Combine(directoryPath, filePath)}");

        _fileStorageMock.Setup(x => x.Read(directoryPath, It.IsAny<string>(), null)).Throws(fileNotFoundException);

        // Act
        Action action = () => snapShotOfflineStorage.Restore(_snapshot.AggregateIdentifier);

        // Assert
        action.Should().ThrowExactly<SnapshotUnboxingFailedException>().WithMessage(
                $"Unexpected error while unboxing snapshot for aggregate with id: ({_snapshot.AggregateIdentifier}). See inner exception for more details.")
            .WithInnerExceptionExactly<FileNotFoundException>().WithMessage(fileNotFoundException.Message);
    }

    [Test]
    public async Task RestoreAsync_WhenAggregateNotFound_ShouldThrowFileNotFoundException()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();
        string filePath = _faker.System.FilePath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileSystemMock.Setup(x => x.Path.Combine(directoryPath, filePath)).Returns(directoryPath + filePath);

        FileNotFoundException fileNotFoundException =
            new($"File not found: {_fileSystemMock.Object.Path.Combine(directoryPath, filePath)}");

        _fileStorageMock.Setup(x => x.ReadAsync(directoryPath, It.IsAny<string>(), null, default))
            .Throws(fileNotFoundException);

        // Act
        Func<Task> action = async () => await snapShotOfflineStorage.RestoreAsync(_snapshot.AggregateIdentifier);

        // Assert
        await action.Should().ThrowExactlyAsync<SnapshotUnboxingFailedException>().WithMessage(
                $"Unexpected error while unboxing snapshot for aggregate with id: ({_snapshot.AggregateIdentifier}). See inner exception for more details.")
            .WithInnerExceptionExactly(fileNotFoundException.GetType()).WithMessage(fileNotFoundException.Message);
    }

    [Test]
    [TestCaseSource(nameof(_possibleExceptions))]
    public void Restore_WhenExceptionOccurs_ShouldThrowAggregateUnBoxingFailedException(Type exception)
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();
        string filePath = _faker.System.FilePath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileSystemMock.Setup(x => x.Path.Combine(directoryPath, filePath)).Returns(directoryPath + filePath);
        _fileStorageMock.Setup(x => x.Read(directoryPath, It.IsAny<string>(), null))
            .Throws(Activator.CreateInstance(exception) as Exception);

        // Act
        Action action = () => snapShotOfflineStorage.Restore(_snapshot.AggregateIdentifier);

        // Assert
        action.Should().ThrowExactly<SnapshotUnboxingFailedException>().WithMessage(
                $"Unexpected error while unboxing snapshot for aggregate with id: ({_snapshot.AggregateIdentifier}). See inner exception for more details.")
            .WithInnerExceptionExactly(exception);
    }

    [Test]
    [TestCaseSource(nameof(_possibleExceptions))]
    public async Task RestoreAsync_WhenExceptionOccurs_ShouldThrowAggregateUnBoxingFailedException(Type exception)
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();
        string filePath = _faker.System.FilePath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _snapshot.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileSystemMock.Setup(x => x.Path.Combine(directoryPath, filePath)).Returns(directoryPath + filePath);
        _fileStorageMock.Setup(x => x.ReadAsync(directoryPath, It.IsAny<string>(), null, default))
            .Throws(Activator.CreateInstance(exception) as Exception);

        // Act
        Func<Task> action = async () => await snapShotOfflineStorage.RestoreAsync(_snapshot.AggregateIdentifier);

        // Assert
        await action.Should().ThrowExactlyAsync<SnapshotUnboxingFailedException>().WithMessage(
                $"Unexpected error while unboxing snapshot for aggregate with id: ({_snapshot.AggregateIdentifier}). See inner exception for more details.")
            .WithInnerExceptionExactly(exception);
    }
}