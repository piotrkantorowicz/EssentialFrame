using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Domain.Events.Core.Factories;
using EssentialFrame.Domain.Events.Exceptions;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Extensions;
using EssentialFrame.Files;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Persistence.Aggregates;

[TestFixture]
public class AggregateOfflineStorageTests
{
    [SetUp]
    public void SetUp()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());
        _logger = NullLoggerFactory.Instance.CreateLogger<AggregateOfflineStorage<PostIdentifier>>();

        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate =
            GenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        _aggregateDataModel = new AggregateDataModel
        {
            AggregateIdentifier = aggregateIdentifier.Identifier,
            AggregateVersion = aggregateVersion,
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent()?.Tenant?.Identifier ?? Guid.Empty
        };

        _domainEvents = new List<IDomainEvent<PostIdentifier>>
        {
            new ChangeTitleDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Title.Default(_faker.Lorem.Sentence())),
            new ChangeDescriptionDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Description.Create(_faker.Lorem.Sentences()))
        };

        _domainEventDataModels = _domainEvents.Select(domainEvent => new DomainEventDataModel
        {
            AggregateIdentifier = domainEvent.AggregateIdentifier.Identifier,
            AggregateVersion = domainEvent.AggregateVersion,
            EventIdentifier = domainEvent.EventIdentifier,
            EventType = domainEvent.GetTypeFullName(),
            EventClass = domainEvent.GetClassName(),
            DomainEvent = _serializerMock.Object.Serialize(domainEvent),
            CreatedAt = domainEvent.EventTime
        }).ToList();
    }

    [TearDown]
    public void TearDown()
    {
        _fileStorageMock.Reset();
        _serializerMock.Reset();
        _fileSystemMock.Reset();
        _identityServiceMock.Reset();
        _domainEventMapperMock.Reset();
        _logger = null;
    }

    private readonly Faker _faker = new();
    private readonly Mock<IFileStorage> _fileStorageMock = new();
    private readonly Mock<ISerializer> _serializerMock = new();
    private readonly Mock<IFileSystem> _fileSystemMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<IDomainEventMapper<PostIdentifier>> _domainEventMapperMock = new();

    private AggregateDataModel _aggregateDataModel;
    private ILogger<AggregateOfflineStorage<PostIdentifier>> _logger;
    private IReadOnlyCollection<DomainEventDataModel> _domainEventDataModels;
    private IReadOnlyCollection<IDomainEvent<PostIdentifier>> _domainEvents;

    private static readonly object[] PossibleExceptions =
    {
        new object[] { typeof(OutOfMemoryException) }, new object[] { typeof(ArgumentException) },
        new object[] { typeof(ArgumentNullException) }, new object[] { typeof(PathTooLongException) },
        new object[] { typeof(DirectoryNotFoundException) }, new object[] { typeof(IOException) },
        new object[] { typeof(UnauthorizedAccessException) }, new object[] { typeof(NotSupportedException) },
        new object[] { typeof(PathTooLongException) }, new object[] { typeof(UnauthorizedAccessException) },
        new object[] { typeof(SecurityException) }
    };

    [Test]
    public void Save_UsingDefaultDirectory_ShouldSaveFile()
    {
        // Arrange
        IAggregateOfflineStorage aggregateOfflineStorage = new AggregateOfflineStorage<PostIdentifier>(
            _fileStorageMock.Object,
            _fileSystemMock.Object, _logger, _domainEventMapperMock.Object, _serializerMock.Object);

        Mock<IFileInfo> fileInfoMock = new();
        string directoryPath = _faker.System.DirectoryPath();

        fileInfoMock.Setup(x => x.Length).Returns(_faker.Random.Long(1, 5000));

        _fileSystemMock.Setup(x =>
                x.Path.Combine(It.IsAny<string>(), _aggregateDataModel.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock.Setup(fs => fs.Create(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null))
            .Returns(fileInfoMock.Object);

        _domainEventMapperMock.Setup(s => s.Map(_domainEventDataModels)).Returns(_domainEvents);
        _serializerMock.Setup(s => s.Serialize(_domainEvents)).Returns(_faker.Lorem.Sentences());

        // Act
        aggregateOfflineStorage.Save(_aggregateDataModel, _domainEventDataModels);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _fileStorageMock.Verify(fs => fs.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null),
            Times.Exactly(3));

        _domainEventMapperMock.Verify(s => s.Map(_domainEventDataModels), Times.Once);
        _serializerMock.Verify(s => s.Serialize(_domainEvents), Times.Once);
    }

    [Test]
    public async Task SaveAsync_UsingDefaultDirectory_ShouldSaveFile()
    {
        // Arrange
        IAggregateOfflineStorage aggregateOfflineStorage = new AggregateOfflineStorage<PostIdentifier>(
            _fileStorageMock.Object,
            _fileSystemMock.Object, _logger, _domainEventMapperMock.Object, _serializerMock.Object);

        Mock<IFileInfo> fileInfoMock = new();
        string directoryPath = _faker.System.DirectoryPath();

        fileInfoMock.Setup(x => x.Length).Returns(_faker.Random.Long(1, 5000));

        _fileSystemMock.Setup(x =>
                x.Path.Combine(It.IsAny<string>(), _aggregateDataModel.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock
            .Setup(fs => fs.CreateAsync(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null, default))
            .ReturnsAsync(fileInfoMock.Object);

        _domainEventMapperMock.Setup(s => s.Map(_domainEventDataModels)).Returns(_domainEvents);
        _serializerMock.Setup(s => s.Serialize(_domainEvents)).Returns(_faker.Lorem.Sentences());

        // Act
        await aggregateOfflineStorage.SaveAsync(_aggregateDataModel, _domainEventDataModels);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _fileStorageMock.Verify(
            fs => fs.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, default),
            Times.Exactly(3));

        _domainEventMapperMock.Verify(s => s.Map(_domainEventDataModels), Times.Once);
        _serializerMock.Verify(s => s.Serialize(_domainEvents), Times.Once);
    }

    [Test]
    public void Save_UsingCustomDirectory_ShouldSaveFile()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        // Arrange
        IAggregateOfflineStorage aggregateOfflineStorage = new AggregateOfflineStorage<PostIdentifier>(
            _fileStorageMock.Object,
            _fileSystemMock.Object, _logger, _domainEventMapperMock.Object, _serializerMock.Object, offlineDirectory);

        Mock<IFileInfo> fileInfoMock = new();
        string directoryPath = _faker.System.DirectoryPath();

        fileInfoMock.Setup(x => x.Length).Returns(_faker.Random.Long(1, 5000));

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _aggregateDataModel.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock.Setup(fs => fs.Create(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null))
            .Returns(fileInfoMock.Object);

        _domainEventMapperMock.Setup(s => s.Map(_domainEventDataModels)).Returns(_domainEvents);
        _serializerMock.Setup(s => s.Serialize(_domainEvents)).Returns(_faker.Lorem.Sentences());

        // Act
        aggregateOfflineStorage.Save(_aggregateDataModel, _domainEventDataModels);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _fileStorageMock.Verify(fs => fs.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null),
            Times.Exactly(3));

        _domainEventMapperMock.Verify(s => s.Map(_domainEventDataModels), Times.Once);
        _serializerMock.Verify(s => s.Serialize(_domainEvents), Times.Once);
    }

    [Test]
    public async Task SaveAsync_UsingCustomDirectory_ShouldSaveFile()
    {
        // Arrange
        string offlineDirectory = _faker.System.DirectoryPath();

        // Arrange
        IAggregateOfflineStorage aggregateOfflineStorage = new AggregateOfflineStorage<PostIdentifier>(
            _fileStorageMock.Object,
            _fileSystemMock.Object, _logger, _domainEventMapperMock.Object, _serializerMock.Object, offlineDirectory);

        Mock<IFileInfo> fileInfoMock = new();
        string directoryPath = _faker.System.DirectoryPath();

        fileInfoMock.Setup(x => x.Length).Returns(_faker.Random.Long(1, 5000));

        _fileSystemMock.Setup(x => x.Path.Combine(offlineDirectory, _aggregateDataModel.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock
            .Setup(fs => fs.CreateAsync(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null, default))
            .ReturnsAsync(fileInfoMock.Object);

        _domainEventMapperMock.Setup(s => s.Map(_domainEventDataModels)).Returns(_domainEvents);
        _serializerMock.Setup(s => s.Serialize(_domainEvents)).Returns(_faker.Lorem.Sentences());

        // Act
        await aggregateOfflineStorage.SaveAsync(_aggregateDataModel, _domainEventDataModels);

        // Assert
        _fileSystemMock.Verify(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        _fileStorageMock.Verify(
            fs => fs.CreateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null, default),
            Times.Exactly(3));

        _domainEventMapperMock.Verify(s => s.Map(_domainEventDataModels), Times.Once);
        _serializerMock.Verify(s => s.Serialize(_domainEvents), Times.Once);
    }

    [Test]
    [TestCaseSource(nameof(PossibleExceptions))]
    public void Save_WhenExceptionOccurs_ShouldCatchAndThrowAggregateBoxingFailedException(Type exception)
    {
        // Arrange
        IAggregateOfflineStorage aggregateOfflineStorage = new AggregateOfflineStorage<PostIdentifier>(
            _fileStorageMock.Object,
            _fileSystemMock.Object, _logger, _domainEventMapperMock.Object, _serializerMock.Object);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x =>
                x.Path.Combine(It.IsAny<string>(), _aggregateDataModel.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock.Setup(fs => fs.Create(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null))
            .Throws(Activator.CreateInstance(exception) as Exception);

        // Act
        Action action = () => aggregateOfflineStorage.Save(_aggregateDataModel, _domainEventDataModels);

        // Assert
        action.Should().ThrowExactly<AggregateBoxingFailedException>().WithMessage(
                $"Unable to box aggregate ({_aggregateDataModel.GetTypeFullName()}) with id: ({_aggregateDataModel.AggregateIdentifier}). See inner exception for more details")
            .WithInnerExceptionExactly(exception);
    }

    [Test]
    [TestCaseSource(nameof(PossibleExceptions))]
    public async Task SaveAsync_WhenExceptionOccurs_ShouldCatchAndThrowAggregateBoxingFailedException(Type exception)
    {
        // Arrange
        IAggregateOfflineStorage aggregateOfflineStorage = new AggregateOfflineStorage<PostIdentifier>(
            _fileStorageMock.Object,
            _fileSystemMock.Object, _logger, _domainEventMapperMock.Object, _serializerMock.Object);

        string directoryPath = _faker.System.DirectoryPath();

        _fileSystemMock.Setup(x =>
                x.Path.Combine(It.IsAny<string>(), _aggregateDataModel.AggregateIdentifier.ToString()))
            .Returns(directoryPath);

        _fileStorageMock
            .Setup(fs => fs.CreateAsync(directoryPath, It.IsAny<string>(), It.IsAny<string>(), null, default))
            .ThrowsAsync(Activator.CreateInstance(exception) as Exception);

        // Act
        Func<Task> action = async () =>
            await aggregateOfflineStorage.SaveAsync(_aggregateDataModel, _domainEventDataModels);

        // Assert
        await action.Should().ThrowExactlyAsync<AggregateBoxingFailedException>().WithMessage(
                $"Unable to box aggregate ({_aggregateDataModel.GetTypeFullName()}) with id: ({_aggregateDataModel.AggregateIdentifier}). See inner exception for more details")
            .WithInnerExceptionExactly(exception);
    }
}