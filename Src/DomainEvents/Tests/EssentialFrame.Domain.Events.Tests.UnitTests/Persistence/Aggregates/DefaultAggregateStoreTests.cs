using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Domain.Events.Core.Factories;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Models;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services;
using EssentialFrame.Domain.Events.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.ExampleApp.Application.Identity;
using EssentialFrame.ExampleApp.Domain.Posts.Aggregates;
using EssentialFrame.ExampleApp.Domain.Posts.DomainEvents;
using EssentialFrame.ExampleApp.Domain.Posts.Entities.Images;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.BytesContents;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Dates;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Descriptions;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Identifiers;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Names;
using EssentialFrame.ExampleApp.Domain.Posts.ValueObjects.Titles;
using EssentialFrame.Extensions;
using EssentialFrame.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Domain.Events.Tests.UnitTests.Persistence.Aggregates;

[TestFixture]
public class DefaultAggregateStoreTests
{
    [SetUp]
    public void SetUp()
    {
        _identityServiceMock.Setup(x => x.GetCurrent()).Returns(new IdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _aggregateCacheMock.Reset();
        _eventsCacheMock.Reset();
        _identityServiceMock.Reset();
        _aggregateOfflineStorageMock.Reset();
        _domainEventMapper.Reset();
        _aggregateMapperMock.Reset();
    }

    private readonly Faker _faker = new();
    private readonly Mock<ICache<Guid, AggregateDataModel>> _aggregateCacheMock = new();
    private readonly Mock<ICache<Guid, DomainEventDataModel>> _eventsCacheMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<IAggregateOfflineStorage> _aggregateOfflineStorageMock = new();
    private readonly Mock<IDomainEventMapper<PostIdentifier>> _domainEventMapper = new();
    private readonly Mock<IAggregateMapper<PostIdentifier>> _aggregateMapperMock = new();

    [Test]
    public void Exists_WhenAggregateIdentifierIsProvided_ShouldReturnTrue()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier.Value)).Returns(true);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = aggregateStore.Exists(aggregateIdentifier.Value);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierIsProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(true);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = await aggregateStore.ExistsAsync(aggregateIdentifier);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Exists_WhenAggregateIdentifierIsProvided_ShouldReturnFalse()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier.Value)).Returns(false);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = aggregateStore.Exists(aggregateIdentifier.Value);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierIsProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        _aggregateCacheMock.Setup(x => x.Exists(aggregateIdentifier)).Returns(false);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = await aggregateStore.ExistsAsync(aggregateIdentifier);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Exists_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnTrue()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateDataModel, bool>>())).Returns(true);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = aggregateStore.Exists(aggregateIdentifier.Value, aggregateVersion);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateDataModel, bool>>())).Returns(true);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = await aggregateStore.ExistsAsync(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Exists_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnFalse()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateDataModel, bool>>())).Returns(false);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = aggregateStore.Exists(aggregateIdentifier.Value, aggregateVersion);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task ExistsAsync_WhenAggregateIdentifierAndVersionAreProvided_ShouldReturnFalse()
    {
        // Arrange
        Guid aggregateIdentifier = Guid.NewGuid();
        int aggregateVersion = _faker.Random.Int();
        _aggregateCacheMock.Setup(x => x.Exists(It.IsAny<Func<Guid, AggregateDataModel, bool>>())).Returns(false);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        bool result = await aggregateStore.ExistsAsync(aggregateIdentifier, aggregateVersion);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Get_WhenAggregateIdentifierIsProvided_ShouldReturnAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());

        AggregateDataModel aggregateDataModel = new()
        {
            AggregateIdentifier = _faker.Random.Guid(),
            AggregateVersion = _faker.Random.Int(),
            DeletedDate = null,
            IsDeleted = false,
            TenantIdentifier = _faker.Random.Guid()
        };

        _aggregateCacheMock.Setup(x => x.Get(aggregateIdentifier.Value)).Returns(aggregateDataModel);

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        AggregateDataModel result = aggregateStore.Get(aggregateIdentifier.Value);

        // Assert
        result.Should().BeEquivalentTo(aggregateDataModel);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIdentifierIsProvided_ShouldReturnAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());

        AggregateDataModel aggregateDataModel = new()
        {
            AggregateIdentifier = _faker.Random.Guid(),
            AggregateVersion = _faker.Random.Int(),
            DeletedDate = _faker.Date.Future(),
            IsDeleted = true,
            TenantIdentifier = _faker.Random.Guid()
        };

        _aggregateCacheMock.Setup(x => x.Get(aggregateIdentifier.Value)).Returns(aggregateDataModel);

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        AggregateDataModel result = await aggregateStore.GetAsync(aggregateIdentifier.Value);

        // Assert
        result.Should().BeEquivalentTo(aggregateDataModel);
    }

    [Test]
    public void Get_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);

        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);
        
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        IReadOnlyCollection<DomainEventDataModel> result =
            aggregateStore.Get(aggregateIdentifier.Value, aggregateVersion);

        // Assert
        result.Should().BeEquivalentTo(domainEventsDms);
    }

    [Test]
    public async Task GetAsync_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);

        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        IReadOnlyCollection<DomainEventDataModel> result =
            await aggregateStore.GetAsync(aggregateIdentifier.Value, aggregateVersion);

        // Assert
        result.Should().BeEquivalentTo(domainEventsDms);
    }

    [Test]
    public void GetDeleted_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        List<AggregateDataModel> aggregates = GenerateAggregates();
        _aggregateCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, AggregateDataModel, bool>>())).Returns(aggregates);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        IEnumerable<Guid> result = aggregateStore.GetDeleted();

        // Assert
        result.Should().BeEquivalentTo(aggregates.Select(a => a.AggregateIdentifier));
    }

    [Test]
    public async Task GetDeletedAsync_WhenAggregateIdentifierIsProvided_ShouldReturnDomainEvents()
    {
        // Arrange
        List<AggregateDataModel> aggregates = GenerateAggregates();
        _aggregateCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, AggregateDataModel, bool>>())).Returns(aggregates);
        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        IEnumerable<Guid> result = await aggregateStore.GetDeletedAsync();

        // Assert
        result.Should().BeEquivalentTo(aggregates.Select(a => a.AggregateIdentifier));
    }

    [Test]
    public void Save_WhenDomainEventsAreProvided_ShouldSaveDomainEvents()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        Post aggregate =
            GenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        AggregateDataModel aggregateDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier.Value,
            AggregateVersion = aggregateVersion,
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);
        IEnumerable<KeyValuePair<Guid, DomainEventDataModel>> domainEventsDictionary =
            domainEventsDms?.Select(v => new KeyValuePair<Guid, DomainEventDataModel>(v.EventIdentifier, v));

        _aggregateCacheMock.Setup(x => x.Add(aggregateIdentifier.Value, aggregateDataModel));

        _eventsCacheMock.Setup(x => x.AddMany(domainEventsDictionary));

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        aggregateStore.Save(aggregateDataModel, domainEventsDms);

        // Assert
        _aggregateCacheMock.Verify(x => x.Add(aggregateIdentifier.Value, aggregateDataModel), Times.Once);
        _eventsCacheMock.Verify(x => x.AddMany(domainEventsDictionary), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenDomainEventsAreProvided_ShouldSaveDomainEvents()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();
        Post aggregate =
            GenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        AggregateDataModel aggregateDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier.Value,
            AggregateVersion = aggregateVersion,
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);
        IEnumerable<KeyValuePair<Guid, DomainEventDataModel>> domainEventsDictionary =
            domainEventsDms?.Select(v => new KeyValuePair<Guid, DomainEventDataModel>(v.EventIdentifier, v));

        _aggregateCacheMock.Setup(x => x.Add(aggregateIdentifier.Value, aggregateDataModel));

        _eventsCacheMock.Setup(x => x.AddMany(domainEventsDictionary));

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        await aggregateStore.SaveAsync(aggregateDataModel, domainEventsDms);

        // Assert
        _aggregateCacheMock.Verify(x => x.Add(aggregateIdentifier.Value, aggregateDataModel), Times.Once);
        _eventsCacheMock.Verify(x => x.AddMany(domainEventsDictionary), Times.Once);
    }

    [Test]
    public void Box_WhenAggregateIsProvided_ShouldReturnAggregateBox()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();

        Post aggregate =
            GenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        AggregateDataModel aggregateDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier.Value,
            AggregateVersion = aggregateVersion,
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);

        _aggregateCacheMock.Setup(x => x.Get(aggregateIdentifier.Value)).Returns(aggregateDataModel);
        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);
        _aggregateOfflineStorageMock.Setup(x => x.Save(aggregateDataModel, domainEventsDms));

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        aggregateStore.Box(aggregateIdentifier.Value);

        // Assert
        _aggregateCacheMock.Verify(x => x.Get(aggregateIdentifier.Value), Times.Once);
        _aggregateOfflineStorageMock.Verify(x => x.Save(aggregateDataModel, domainEventsDms), Times.Once);
        _eventsCacheMock.Verify(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()), Times.Once);
    }

    [Test]
    public async Task BoxAsync_WhenAggregateIsProvided_ShouldReturnAggregateBox()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        int aggregateVersion = _faker.Random.Int();

        Post aggregate =
            GenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier, aggregateVersion);

        AggregateDataModel aggregateDataModel = new()
        {
            AggregateIdentifier = aggregateIdentifier.Value,
            AggregateVersion = aggregateVersion,
            DeletedDate = aggregate.DeletedDate,
            IsDeleted = aggregate.IsDeleted,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<DomainEventDataModel> domainEventsDms = GenerateDomainEventsCollection(aggregateIdentifier);

        _aggregateCacheMock.Setup(x => x.Get(aggregateIdentifier.Value)).Returns(aggregateDataModel);
        _eventsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()))
            .Returns(domainEventsDms);
        _aggregateOfflineStorageMock.Setup(x => x.SaveAsync(aggregateDataModel, domainEventsDms, default));

        DefaultAggregateStore aggregateStore = new(_eventsCacheMock.Object, _aggregateCacheMock.Object,
            _aggregateOfflineStorageMock.Object);

        // Act
        await aggregateStore.BoxAsync(aggregateIdentifier.Value);

        // Assert
        _aggregateCacheMock.Verify(x => x.Get(aggregateIdentifier.Value), Times.Once);
        _aggregateOfflineStorageMock.Verify(x => x.SaveAsync(aggregateDataModel, domainEventsDms, default), Times.Once);
        _eventsCacheMock.Verify(x => x.GetMany(It.IsAny<Func<Guid, DomainEventDataModel, bool>>()), Times.Once);
    }

    private List<AggregateDataModel> GenerateAggregates()
    {
        List<Post> aggregates = new()
        {
            GenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(PostIdentifier.New(_faker.Random.Guid()),
                _faker.Random.Int(1, 1000)),
            GenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(PostIdentifier.New(_faker.Random.Guid()),
                _faker.Random.Int(1, 1000)),
            GenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(PostIdentifier.New(_faker.Random.Guid()),
                _faker.Random.Int(1, 1000))
        };

        List<AggregateDataModel> aggregateDataModels = aggregates.Select(a => new AggregateDataModel
        {
            AggregateIdentifier = a.AggregateIdentifier.Value,
            AggregateVersion = a.AggregateVersion,
            DeletedDate = a.DeletedDate,
            IsDeleted = a.IsDeleted,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        }).ToList();

        return aggregateDataModels;
    }

    private List<DomainEventDataModel> GenerateDomainEventsCollection(PostIdentifier aggregateIdentifier)
    {
        List<IDomainEvent<PostIdentifier>> domainEvents = new()
        {
            new ChangeDescriptionDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Description.Create(_faker.Lorem.Sentences())),
            new ChangeTitleDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Title.Default(_faker.Lorem.Sentence())),
            new ChangeDescriptionDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Description.Create(_faker.Lorem.Sentences())),
            new ChangeTitleDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Title.Default(_faker.Lorem.Sentence())),
            new ChangeTitleDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Title.Default(_faker.Lorem.Sentence())),
            new AddImagesDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                new HashSet<Image>
                {
                    Image.Create(_faker.Random.Guid(),
                        Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                        BytesContent.Create(_faker.Random.Bytes(389)))
                }),
            new ChangeDescriptionDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Description.Create(_faker.Lorem.Sentences())),
            new AddImagesDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                new HashSet<Image>
                {
                    Image.Create(_faker.Random.Guid(),
                        Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                        BytesContent.Create(_faker.Random.Bytes(2346))),
                    Image.Create(_faker.Random.Guid(),
                        Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                        BytesContent.Create(_faker.Random.Bytes(982)))
                }),
            new ChangeImageNameDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                _faker.Random.Guid(), Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)))),
            new ChangeExpirationDateDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(),
                Date.Create(_faker.Date.Future()))
        };

        List<DomainEventDataModel> domainEventDms = domainEvents.Select(e => new DomainEventDataModel
        {
            AggregateIdentifier = e.AggregateIdentifier.Value,
            AggregateVersion = e.AggregateVersion,
            EventIdentifier = e.EventIdentifier,
            EventType = e.GetTypeFullName(),
            EventClass = e.GetClassName(),
            DomainEvent = e,
            CreatedAt = e.EventTime
        }).ToList();

        return _faker.Random.ListItems(domainEventDms, _faker.Random.Int(1, 10)).ToList();
    }
}