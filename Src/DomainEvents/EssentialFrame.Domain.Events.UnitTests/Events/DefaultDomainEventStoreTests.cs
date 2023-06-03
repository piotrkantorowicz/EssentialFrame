using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Aggregates;
using EssentialFrame.Domain.Events.Events;
using EssentialFrame.Domain.Events.Events.Interfaces;
using EssentialFrame.Domain.Factories;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.TestData.Domain.Aggregates;
using EssentialFrame.TestData.Domain.DomainEvents;
using EssentialFrame.TestData.Domain.Entities;
using EssentialFrame.TestData.Domain.ValueObjects;
using EssentialFrame.TestData.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.UnitTests.Events;

[TestFixture]
public class DefaultDomaindomainEventStoreTests
{
    private readonly Faker _faker = new();
    private Mock<ICache<Guid, AggregateRoot>> _aggregateCacheMock;
    private Mock<ICache<Guid, DomainEventDataModel>> _eventsCacheMock;
    private Mock<IIdentityService> _identityServiceMock;
    private Mock<IAggregateOfflineStorage> _aggregateOfflineStorageMock;
    private Mock<ISerializer> _serializerMock;

    [SetUp]
    public void SetUp()
    {
        _aggregateCacheMock = new Mock<ICache<Guid, AggregateRoot>>();
        _eventsCacheMock = new Mock<ICache<Guid, DomainEventDataModel>>();
        _identityServiceMock = new Mock<IIdentityService>();
        _aggregateOfflineStorageMock = new Mock<IAggregateOfflineStorage>();
        _serializerMock = new Mock<ISerializer>();

        _identityServiceMock.Setup(x => x.GetCurrent()).Returns(new TestIdentity());
    }

    [TearDown]
    public void TearDown()
    {
        _aggregateCacheMock.Reset();
        _eventsCacheMock.Reset();
        _identityServiceMock.Reset();
        _aggregateOfflineStorageMock.Reset();
        _serializerMock.Reset();
    }

    [Test]
    public void Exists_WhenAggregateIdentifierIsProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(true);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        bool result = domainEventStore.Exists(aggregateIdentifier);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierIsProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(true);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        bool result = await domainEventStore.ExistsAsync(aggregateIdentifier);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Exists_WhenAggregateIdentifierIsProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(false);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        bool result = domainEventStore.Exists(aggregateIdentifier);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierIsProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(false);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        bool result = await domainEventStore.ExistsAsync(aggregateIdentifier);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Exists_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateRoot, bool>>())).Returns(true);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        bool result = domainEventStore.Exists(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateRoot, bool>>())).Returns(true);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        bool result = await domainEventStore.ExistsAsync(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Exists_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateRoot, bool>>())).Returns(false);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        bool result = domainEventStore.Exists(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateRoot, bool>>())).Returns(false);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        bool result = await domainEventStore.ExistsAsync(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Get_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        List<IDomainEvent> domainEvents = GenerateEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> domainEventsDms = domainEvents.Select(x => new DomainEventDataModel(x)).ToList();

        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        IReadOnlyCollection<DomainEventDataModel> result = domainEventStore.Get(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeEquivalentTo(domainEventsDms);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        int aggregateVersion = _faker.Random.Int();
        List<IDomainEvent> domainEvents = GenerateEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> domainEventsDms = domainEvents.Select(x => new DomainEventDataModel(x)).ToList();

        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);

        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        IReadOnlyCollection<DomainEventDataModel> result =
            await domainEventStore.GetAsync(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeEquivalentTo(domainEventsDms);
    }

    [Test]
    public void GetDeleted_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        List<AggregateRoot> aggregates = GenerateAggregates();
        _aggregateCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, AggregateRoot, bool>>())).Returns(aggregates);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        IEnumerable<Guid> result = domainEventStore.GetDeleted();

        // Assert
        result.Should().BeEquivalentTo(aggregates.Select(a => a.AggregateIdentifier));
    }

    [Test]
    public async Task GetDeletedAsync_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        List<AggregateRoot> aggregates = GenerateAggregates();
        _aggregateCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, AggregateRoot, bool>>())).Returns(aggregates);
        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        IEnumerable<Guid> result = await domainEventStore.GetDeletedAsync();

        // Assert
        result.Should().BeEquivalentTo(aggregates.Select(a => a.AggregateIdentifier));
    }

    [Test]
    public void Save_WhenDomainEventsAreProvided_ShouldSaveDomainEvents()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        TestAggregate aggregate = GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);
        List<IDomainEvent> domainEvents = GenerateEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> domainEventsDms = domainEvents?.Select(x => new DomainEventDataModel(x)).ToList();
        IEnumerable<KeyValuePair<Guid, DomainEventDataModel>> domainEventsDictionary =
            domainEventsDms?.Select(v => new KeyValuePair<Guid, DomainEventDataModel>(v.EventIdentifier, v));

        _aggregateCacheMock.Setup(x => x.Add(aggregateIdentifier, aggregate));

        _eventsCacheMock.Setup(x => x.AddMany(domainEventsDictionary));

        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        domainEventStore.Save(aggregate, domainEventsDms);

        // Assert
        _aggregateCacheMock.Verify(x => x.Add(aggregateIdentifier, aggregate), Times.Once);
        _eventsCacheMock.Verify(x => x.AddMany(domainEventsDictionary), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenDomainEventsAreProvided_ShouldSaveDomainEvents()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        int aggregateVersion = _faker.Random.Int();
        TestAggregate aggregate = GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);
        List<IDomainEvent> domainEvents = GenerateEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> domainEventsDms = domainEvents?.Select(x => new DomainEventDataModel(x)).ToList();
        IEnumerable<KeyValuePair<Guid, DomainEventDataModel>> domainEventsDictionary =
            domainEventsDms?.Select(v => new KeyValuePair<Guid, DomainEventDataModel>(v.EventIdentifier, v));

        _aggregateCacheMock.Setup(x => x.Add(aggregateIdentifier, aggregate));

        _eventsCacheMock.Setup(x => x.AddMany(domainEventsDictionary));

        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        await domainEventStore.SaveAsync(aggregate, domainEventsDms);

        // Assert
        _aggregateCacheMock.Verify(x => x.Add(aggregateIdentifier, aggregate), Times.Once);
        _eventsCacheMock.Verify(x => x.AddMany(domainEventsDictionary), Times.Once);
    }

    [Test]
    public void Box_WhenAggregateIsProvided_ShouldReturnAggregateBox()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        TestAggregate aggregate = GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);
        List<IDomainEvent> domainEvents = GenerateEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> domainEventsDms = domainEvents?.Select(x => new DomainEventDataModel(x)).ToList();

        _aggregateCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(aggregate);
        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);
        _aggregateOfflineStorageMock.Setup(x => x.Save(aggregate, domainEvents));

        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        domainEventStore.Box(aggregateIdentifier);

        // Assert
        _aggregateCacheMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
        _aggregateOfflineStorageMock.Verify(x => x.Save(aggregate, domainEvents), Times.Once);
        _eventsCacheMock.Verify(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()), Times.Once);
    }

    [Test]
    public async Task BoxAsync_WhenAggregateIsProvided_ShouldReturnAggregateBox()
    {
        // Arrange
        Guid aggregateIdentifier = _faker.Random.Guid();
        int aggregateVersion = _faker.Random.Int();
        TestAggregate aggregate = GenericAggregateFactory<TestAggregate>.CreateAggregate(aggregateIdentifier,
            aggregateVersion, _identityServiceMock.Object);
        List<IDomainEvent> domainEvents = GenerateEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> domainEventsDms = domainEvents?.Select(x => new DomainEventDataModel(x)).ToList();

        _aggregateCacheMock.Setup(x => x.Get(aggregateIdentifier)).Returns(aggregate);
        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);
        _aggregateOfflineStorageMock.Setup(x => x.SaveAsync(aggregate, domainEvents, default));

        DefaultDomainEventsStore domainEventStore = new(_eventsCacheMock.Object,
            _aggregateCacheMock.Object, _aggregateOfflineStorageMock.Object, _serializerMock.Object);

        // Act
        await domainEventStore.BoxAsync(aggregateIdentifier);

        // Assert
        _aggregateCacheMock.Verify(x => x.Get(aggregateIdentifier), Times.Once);
        _aggregateOfflineStorageMock.Verify(x => x.SaveAsync(aggregate, domainEvents, default), Times.Once);
        _eventsCacheMock.Verify(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()), Times.Once);
    }

    private List<AggregateRoot> GenerateAggregates()
    {
        List<AggregateRoot> aggregates = new()
        {
            GenericAggregateFactory<TestAggregate>.CreateAggregate(_faker.Random.Guid(), _faker.Random.Int(1, 1000),
                _identityServiceMock.Object),
            GenericAggregateFactory<TestAggregate>.CreateAggregate(_faker.Random.Guid(), _faker.Random.Int(1, 1000),
                _identityServiceMock.Object),
            GenericAggregateFactory<TestAggregate>.CreateAggregate(_faker.Random.Guid(), _faker.Random.Int(1, 1000),
                _identityServiceMock.Object)
        };

        return aggregates;
    }

    private List<IDomainEvent> GenerateEventsCollection(Guid aggregateIdentifier)
    {
        List<IDomainEvent> domainEvents = new()
        {
            new ChangeDescriptionTestDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                _faker.Lorem.Sentences()),
            new ChangeTitleTestDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                TestTitle.Create(_faker.Random.Word(), _faker.Random.Bool())),
            new ChangeDescriptionTestDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                _faker.Lorem.Sentences()),
            new ChangeTitleTestDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                TestTitle.Create(_faker.Random.Word(), _faker.Random.Bool())),
            new ChangeTitleTestDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                TestTitle.Create(_faker.Random.Word(), _faker.Random.Bool())),
            new AddImagesDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                new HashSet<TestEntity>
                {
                    TestEntity.Create(_faker.Random.Guid(), _faker.Random.Word(), _faker.Random.Bytes(389))
                }),
            new ChangeDescriptionTestDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                _faker.Lorem.Sentences()),
            new AddImagesDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                new HashSet<TestEntity>
                {
                    TestEntity.Create(_faker.Random.Guid(), _faker.Random.Word(), _faker.Random.Bytes(2346)),
                    TestEntity.Create(_faker.Random.Guid(), _faker.Random.Word(), _faker.Random.Bytes(982))
                }),
            new ChangeImageNameDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                _faker.Random.Guid(), _faker.Lorem.Word()),
            new ChangeExpirationDateTestDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                _faker.Date.Future())
        };

        return _faker.Random.ListItems(domainEvents, _faker.Random.Int(1, 10)).ToList();
    }
}