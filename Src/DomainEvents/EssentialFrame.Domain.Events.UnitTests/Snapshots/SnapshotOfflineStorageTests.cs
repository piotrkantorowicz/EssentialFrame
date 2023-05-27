using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Security;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Snapshots;
using EssentialFrame.Domain.Events.Snapshots.Interfaces;
using EssentialFrame.Domain.Factories;
using EssentialFrame.Domain.Snapshots;
using EssentialFrame.Extensions;
using EssentialFrame.Files;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.TestData.Domain.Aggregates;
using EssentialFrame.TestData.Domain.DomainEvents;
using EssentialFrame.TestData.Domain.ValueObjects;
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
    private AggregateRoot _aggregate;

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
    public void Setup()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new TestIdentity());
        _logger = NullLoggerFactory.Instance.CreateLogger<SnapshotOfflineStorage>();
        Guid aggregateIdentifier = _faker.Random.Guid();
        const int aggregateVersion = 0;

        _aggregate = GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier, aggregateVersion,
            _identityServiceMock.Object);

        List<IDomainEvent> events = new()
        {
            new ChangeTitleTestDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                aggregateVersion + 1, TestTitle.Create(_faker.Lorem.Sentence(), true)),
            new ChangeDescriptionTestDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                aggregateVersion + 2, _faker.Lorem.Sentences())
        };

        _aggregate.Rehydrate(events);
    }

    [TearDown]
    public void Destroy()
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

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _serializerMock.Setup(x => x.Serialize(_aggregate.State)).Returns(serializedState);

        // Act
        snapShotOfflineStorage.Save(_aggregate);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()),
            Times.Once);

        _serializerMock.Verify(x => x.Serialize(_aggregate.State), Times.Once);
        _fileStorageMock.Verify(x => x.Create(directoryPath, It.IsAny<string>(), serializedState, null), Times.Once);
    }

    [Test]
    public async Task SaveAsync_UsingDefaultDirectory_ShouldSaveFile()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _serializerMock.Setup(x => x.Serialize(_aggregate.State)).Returns(serializedState);

        // Act
        await snapShotOfflineStorage.SaveAsync(_aggregate);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()),
            Times.Once);

        _serializerMock.Verify(x => x.Serialize(_aggregate.State), Times.Once);
        _fileStorageMock.Verify(x => x.CreateAsync(directoryPath, It.IsAny<string>(), serializedState, null, default),
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

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _serializerMock.Setup(x => x.Serialize(_aggregate.State)).Returns(serializedState);

        // Act
        snapShotOfflineStorage.Save(_aggregate);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()),
            Times.Once);

        _serializerMock.Verify(x => x.Serialize(_aggregate.State), Times.Once);
        _fileStorageMock.Verify(x => x.Create(directoryPath, It.IsAny<string>(), serializedState, null), Times.Once);
    }

    [Test]
    public async Task SaveAsync_UsingCustomDirectory_ShouldSaveFile()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, offlineDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _serializerMock.Setup(x => x.Serialize(_aggregate.State)).Returns(serializedState);

        // Act
        await snapShotOfflineStorage.SaveAsync(_aggregate);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()),
            Times.Once);

        _serializerMock.Verify(x => x.Serialize(_aggregate.State), Times.Once);
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

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _serializerMock.Setup(x => x.Serialize(_aggregate.State)).Returns(_faker.Lorem.Sentences());

        _fileStorageMock.Setup(x => x.Create(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null))
            .Throws(Activator.CreateInstance(exception) as Exception);

        // Act
        Action action = () => snapShotOfflineStorage.Save(_aggregate);

        // Assert
        action.Should().ThrowExactly<AggregateBoxingFailedException>().WithMessage(
                $"Unable to box aggregate ({_aggregate.GetTypeFullName()}) with id: ({_aggregate.AggregateIdentifier}). See inner exception for more details.")
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

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _serializerMock.Setup(x => x.Serialize(_aggregate.State)).Returns(_faker.Lorem.Sentences());

        _fileStorageMock.Setup(x => x.CreateAsync(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null, default))
            .Throws(Activator.CreateInstance(exception) as Exception);

        // Act
        Func<Task> action = async () => await snapShotOfflineStorage.SaveAsync(_aggregate);

        // Assert
        await action.Should().ThrowExactlyAsync<AggregateBoxingFailedException>().WithMessage(
                $"Unable to box aggregate ({_aggregate.GetTypeFullName()}) with id: ({_aggregate.AggregateIdentifier}). See inner exception for more details.")
            .WithInnerExceptionExactly(exception);
    }

    [Test]
    public void Restore_UsingDefaultDirectory_ShouldRestoreAggregate()
    {
        // Arrange
        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _fileStorageMock.Setup(x => x.Read(directoryPath, It.IsAny<string>(), null)).Returns(serializedState);

        // Act
        Snapshot snapshot = snapShotOfflineStorage.Restore(_aggregate.AggregateIdentifier);

        // Assert
        snapshot.AggregateIdentifier.Should().Be(_aggregate.AggregateIdentifier);
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

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _fileStorageMock.Setup(x => x.ReadAsync(directoryPath, It.IsAny<string>(), null, default))
            .ReturnsAsync(serializedState);

        // Act
        Snapshot snapshot = await snapShotOfflineStorage.RestoreAsync(_aggregate.AggregateIdentifier);

        // Assert
        snapshot.AggregateIdentifier.Should().Be(_aggregate.AggregateIdentifier);
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

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _fileStorageMock.Setup(x => x.Read(directoryPath, It.IsAny<string>(), null)).Returns(serializedState);

        // Act
        Snapshot snapshot = snapShotOfflineStorage.Restore(_aggregate.AggregateIdentifier);

        // Assert
        snapshot.AggregateIdentifier.Should().Be(_aggregate.AggregateIdentifier);
        snapshot.AggregateVersion.Should().Be(Defaults.DefaultAggregateVersion);
        snapshot.AggregateState.Should().Be(serializedState);
    }

    [Test]
    public void RestoreAsync_UningCustomDirectory_ShouldRestoreAggregate()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        ISnapshotOfflineStorage snapShotOfflineStorage = new SnapshotOfflineStorage(_fileStorageMock.Object,
            _serializerMock.Object, _fileSystemMock.Object, _logger, offlineDirectory);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        string serializedState = _faker.Lorem.Sentences();

        _fileStorageMock.Setup(x => x.ReadAsync(directoryPath, It.IsAny<string>(), null, default))
            .ReturnsAsync(serializedState);

        // Act
        Snapshot snapshot = snapShotOfflineStorage.RestoreAsync(_aggregate.AggregateIdentifier).GetAwaiter()
            .GetResult();

        // Assert
        snapshot.AggregateIdentifier.Should().Be(_aggregate.AggregateIdentifier);
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

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileSystemMock.Setup(x => x.Path.Combine(directoryPath, filePath)).Returns(directoryPath + filePath);

        FileNotFoundException fileNotFoundException =
            new($"File not found: {_fileSystemMock.Object.Path.Combine(directoryPath, filePath)}");

        _fileStorageMock.Setup(x => x.Read(directoryPath, It.IsAny<string>(), null)).Throws(fileNotFoundException);

        // Act
        Action action = () => snapShotOfflineStorage.Restore(_aggregate.AggregateIdentifier);

        // Assert
        action.Should().ThrowExactly<AggregateUnBoxingFailedException>().WithMessage(
                $"Unable to unbox aggregate with id: ({_aggregate.AggregateIdentifier}). See inner exception for more details.")
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

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileSystemMock.Setup(x => x.Path.Combine(directoryPath, filePath)).Returns(directoryPath + filePath);

        FileNotFoundException fileNotFoundException =
            new($"File not found: {_fileSystemMock.Object.Path.Combine(directoryPath, filePath)}");

        _fileStorageMock.Setup(x => x.ReadAsync(directoryPath, It.IsAny<string>(), null, default))
            .Throws(fileNotFoundException);

        // Act
        Func<Task> action = async () => await snapShotOfflineStorage.RestoreAsync(_aggregate.AggregateIdentifier);

        // Assert
        await action.Should().ThrowExactlyAsync<AggregateUnBoxingFailedException>().WithMessage(
                $"Unable to unbox aggregate with id: ({_aggregate.AggregateIdentifier}). See inner exception for more details.")
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

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileSystemMock.Setup(x => x.Path.Combine(directoryPath, filePath)).Returns(directoryPath + filePath);
        _fileStorageMock.Setup(x => x.Read(directoryPath, It.IsAny<string>(), null))
            .Throws(Activator.CreateInstance(exception) as Exception);

        // Act
        Action action = () => snapShotOfflineStorage.Restore(_aggregate.AggregateIdentifier);

        // Assert
        action.Should().ThrowExactly<AggregateUnBoxingFailedException>().WithMessage(
                $"Unable to unbox aggregate with id: ({_aggregate.AggregateIdentifier}). See inner exception for more details.")
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

        _fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), _aggregate.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileSystemMock.Setup(x => x.Path.Combine(directoryPath, filePath)).Returns(directoryPath + filePath);
        _fileStorageMock.Setup(x => x.ReadAsync(directoryPath, It.IsAny<string>(), null, default))
            .Throws(Activator.CreateInstance(exception) as Exception);

        // Act
        Func<Task> action = async () => await snapShotOfflineStorage.RestoreAsync(_aggregate.AggregateIdentifier);

        // Assert
        await action.Should().ThrowExactlyAsync<AggregateUnBoxingFailedException>().WithMessage(
                $"Unable to unbox aggregate with id: ({_aggregate.AggregateIdentifier}). See inner exception for more details.")
            .WithInnerExceptionExactly(exception);
    }
}