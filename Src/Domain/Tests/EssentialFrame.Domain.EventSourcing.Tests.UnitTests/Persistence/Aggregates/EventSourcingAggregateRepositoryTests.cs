using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Domain.Core.Events;
using EssentialFrame.Domain.EventSourcing.Core.Factories;
using EssentialFrame.Domain.EventSourcing.Exceptions;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Mappers.Interfaces;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Models;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services;
using EssentialFrame.Domain.EventSourcing.Persistence.Aggregates.Services.Interfaces;
using EssentialFrame.Domain.Persistence.Mappers;
using EssentialFrame.Domain.Persistence.Mappers.Interfaces;
using EssentialFrame.Domain.Persistence.Models;
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

namespace EssentialFrame.Domain.EventSourcing.Tests.UnitTests.Persistence.Aggregates;

[TestFixture]
public class EventSourcingAggregateRepositoryTests
{
    private readonly Faker _faker = new();
    private readonly Mock<IEventSourcingAggregateMapper<PostIdentifier>> _aggregateMapperMock = new();
    private readonly Mock<IDomainEventMapper<PostIdentifier>> _domainEventMapperMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<IEventSourcingAggregateStore> _aggregateStoreMock = new();

    [SetUp]
    public void SetUp()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _domainEventMapperMock.Reset();
        _identityServiceMock.Reset();
        _aggregateStoreMock.Reset();
        _aggregateMapperMock.Reset();
    }

    [Test]
    public void Get_WhenAggregateExists_ReturnsAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier.Value,
            AggregateVersion = aggregate.AggregateVersion,
            DeletedDate = null,
            IsDeleted = false,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<DomainEventDataModel> eventDataModels = GenerateDomainEventDataModelsCollection(aggregateIdentifier);

        aggregate.Rehydrate(GenerateDomainEventsCollection(aggregateIdentifier));

        _aggregateStoreMock.Setup(x => x.Get(aggregateIdentifier.Value, -1)).Returns(eventDataModels);
        _domainEventMapperMock.Setup(x => x.Map(eventDataModels))
            .Returns(GenerateDomainEventsCollection(aggregateIdentifier));
        _aggregateStoreMock.Setup(x => x.Get(aggregateIdentifier.Value)).Returns(eventSourcingAggregateDataModel);

        IEventSourcingAggregateRepository<Post, PostIdentifier> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object);

        // Act
        Post result = eventSourcingAggregateRepository.Get(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);

        _aggregateStoreMock.Verify(x => x.Get(aggregateIdentifier.Value, -1), Times.Once);
        _domainEventMapperMock.Verify(x => x.Map(eventDataModels), Times.Once);
        _aggregateStoreMock.Verify(x => x.Get(aggregateIdentifier.Value), Times.Once);
    }

    [Test]
    public void Get_WhenAggregateHasBeenDeleted_ShouldThrowAggregateDeletedException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier.Value,
            AggregateVersion = aggregate.AggregateVersion,
            DeletedDate = _faker.Date.PastOffset(),
            IsDeleted = true,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<IDomainEvent<PostIdentifier>> events = GenerateDomainEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> eventDataModels = GenerateDomainEventDataModelsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _aggregateStoreMock.Setup(x => x.Get(aggregateIdentifier.Value, -1)).Returns(eventDataModels);
        _aggregateStoreMock.Setup(x => x.Get(aggregateIdentifier.Value)).Returns(eventSourcingAggregateDataModel);

        IEventSourcingAggregateRepository<Post, PostIdentifier> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object);

        // Act
        Action getAggregateAction = () => eventSourcingAggregateRepository.Get(aggregateIdentifier);

        // Assert
        getAggregateAction.Should().Throw<AggregateDeletedException>().WithMessage(
            $"Unable to get aggregate ({aggregate.GetTypeFullName()}) with id: ({aggregateIdentifier}), because it has been deleted");

        _aggregateStoreMock.Verify(x => x.Get(aggregateIdentifier.Value, -1), Times.Once);
        _aggregateStoreMock.Verify(x => x.Get(aggregateIdentifier.Value), Times.Once);
    }

    [Test]
    public void Get_WhenAggregateEventsNotFound_ShouldThrowAggregateNotFoundException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier.Value,
            AggregateVersion = aggregate.AggregateVersion,
            DeletedDate = null,
            IsDeleted = false,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<IDomainEvent<PostIdentifier>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _aggregateStoreMock.Setup(x => x.Get(aggregateIdentifier.Value, -1))
            .Returns((IReadOnlyCollection<DomainEventDataModel>)null);

        _aggregateStoreMock.Setup(x => x.Get(aggregateIdentifier.Value)).Returns(eventSourcingAggregateDataModel);

        IEventSourcingAggregateRepository<Post, PostIdentifier> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object);

        // Act
        Action getAggregateAction = () => eventSourcingAggregateRepository.Get(aggregateIdentifier);

        // Assert
        getAggregateAction.Should().Throw<AggregateNotFoundException>().WithMessage(
            $"This aggregate does not exist ({aggregate.GetTypeFullName()} {aggregateIdentifier}) because there are no events for it");

        _aggregateStoreMock.Verify(x => x.Get(aggregateIdentifier.Value, -1), Times.Once);
        _aggregateStoreMock.Verify(x => x.Get(aggregateIdentifier.Value), Times.Once);
    }

    [Test]
    public async Task GetAsync_WhenAggregateExists_ReturnsAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier.Value,
            AggregateVersion = aggregate.AggregateVersion,
            DeletedDate = null,
            IsDeleted = false,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<DomainEventDataModel> eventDataModels = GenerateDomainEventDataModelsCollection(aggregateIdentifier);

        aggregate.Rehydrate(GenerateDomainEventsCollection(aggregateIdentifier));

        _aggregateStoreMock.Setup(x => x.GetAsync(aggregateIdentifier.Value, -1, default))
            .ReturnsAsync(eventDataModels);
        _aggregateStoreMock.Setup(x => x.GetAsync(aggregateIdentifier.Value, default))
            .ReturnsAsync(eventSourcingAggregateDataModel);
        _domainEventMapperMock.Setup(x => x.Map(eventDataModels))
            .Returns(GenerateDomainEventsCollection(aggregateIdentifier));

        IEventSourcingAggregateRepository<Post, PostIdentifier> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object);

        // Act
        Post result = await eventSourcingAggregateRepository.GetAsync(aggregateIdentifier);

        // Assert
        result.Should().BeEquivalentTo(aggregate);

        _aggregateStoreMock.Verify(x => x.GetAsync(aggregateIdentifier.Value, -1, default), Times.Once);
        _domainEventMapperMock.Verify(x => x.Map(eventDataModels), Times.Once);
        _aggregateStoreMock.Verify(x => x.GetAsync(aggregateIdentifier.Value, default), Times.Once);
    }

    [Test]
    public async Task GetAsync_WhenAggregateHasBeenDeleted_ShouldThrowAggregateDeletedException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier.Value,
            AggregateVersion = aggregate.AggregateVersion,
            DeletedDate = _faker.Date.PastOffset(),
            IsDeleted = true,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<IDomainEvent<PostIdentifier>> events = GenerateDomainEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> eventDataModels = GenerateDomainEventDataModelsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _aggregateStoreMock.Setup(x => x.GetAsync(aggregateIdentifier.Value, -1, default))
            .ReturnsAsync(eventDataModels);
        _aggregateStoreMock.Setup(x => x.GetAsync(aggregateIdentifier.Value, default))
            .ReturnsAsync(eventSourcingAggregateDataModel);

        IEventSourcingAggregateRepository<Post, PostIdentifier> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object);

        // Act
        Func<Task> getAggregateAction =
            async () => await eventSourcingAggregateRepository.GetAsync(aggregateIdentifier);

        // Assert
        await getAggregateAction.Should().ThrowAsync<AggregateDeletedException>().WithMessage(
            $"Unable to get aggregate ({aggregate.GetTypeFullName()}) with id: ({aggregateIdentifier}), because it has been deleted");

        _aggregateStoreMock.Verify(x => x.GetAsync(aggregateIdentifier.Value, -1, default), Times.Once);
        _aggregateStoreMock.Verify(x => x.GetAsync(aggregateIdentifier.Value, default), Times.Once);
    }

    [Test]
    public async Task GetAsync_WhenAggregateEventsNotFound_ShouldThrowAggregateNotFoundException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier.Value,
            AggregateVersion = aggregate.AggregateVersion,
            DeletedDate = null,
            IsDeleted = false,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        List<IDomainEvent<PostIdentifier>> events = GenerateDomainEventsCollection(aggregateIdentifier);

        aggregate.Rehydrate(events);

        _aggregateStoreMock.Setup(x => x.GetAsync(aggregateIdentifier.Value, -1, default))
            .ReturnsAsync((IReadOnlyCollection<DomainEventDataModel>)null);
        _aggregateStoreMock.Setup(x => x.GetAsync(aggregateIdentifier.Value, default))
            .ReturnsAsync(eventSourcingAggregateDataModel);

        IEventSourcingAggregateRepository<Post, PostIdentifier> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object);

        // Act
        Func<Task> getAggregateAction =
            async () => await eventSourcingAggregateRepository.GetAsync(aggregateIdentifier);

        // Assert
        await getAggregateAction.Should().ThrowAsync<AggregateNotFoundException>().WithMessage(
            $"This aggregate does not exist ({aggregate.GetTypeFullName()} {aggregateIdentifier}) because there are no events for it");

        _aggregateStoreMock.Verify(x => x.GetAsync(aggregateIdentifier.Value, -1, default), Times.Once);
        _aggregateStoreMock.Verify(x => x.GetAsync(aggregateIdentifier.Value, default), Times.Once);
    }

    [Test]
    public void Save_WhenCorrectAggregateProvided_ShouldSaveAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null, _identityServiceMock.Object.GetCurrent());

        aggregate.ChangeDescription(Description.Create(_faker.Lorem.Sentences()),
            _identityServiceMock.Object.GetCurrent());
        aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()), _identityServiceMock.Object.GetCurrent());
        aggregate.ExtendExpirationDate(Date.Create(_faker.Date.FutureOffset()),
            _identityServiceMock.Object.GetCurrent());

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier.Value,
            AggregateVersion = aggregate.AggregateVersion,
            DeletedDate = null,
            IsDeleted = false,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        IDomainEvent<PostIdentifier>[] events = aggregate.GetUncommittedChanges();
        List<DomainEventDataModel> eventDataModels =
            GenerateDomainEventDataModelsCollection(aggregateIdentifier, events);

        _aggregateMapperMock.Setup(x => x.Map(aggregate)).Returns(eventSourcingAggregateDataModel);
        _domainEventMapperMock.Setup(x => x.Map(events)).Returns(eventDataModels);
        _aggregateStoreMock.Setup(x => x.Save(eventSourcingAggregateDataModel, eventDataModels));

        IEventSourcingAggregateRepository<Post, PostIdentifier> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object);

        // Act
        IDomainEvent<PostIdentifier>[] domainEvents = eventSourcingAggregateRepository.Save(aggregate);

        // Assert
        domainEvents.Should().BeEquivalentTo(events);
        _aggregateMapperMock.Verify(x => x.Map(aggregate), Times.Once());
        _domainEventMapperMock.Verify(x => x.Map(events), Times.Once());
        _aggregateStoreMock.Verify(x => x.Save(eventSourcingAggregateDataModel, eventDataModels), Times.Once());
    }

    [Test]
    public void Save_WhenNextVersionIsOccupied_ShouldThrowConcurrencyException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;
        const int expectedAggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null, _identityServiceMock.Object.GetCurrent());

        aggregate.ChangeDescription(Description.Create(_faker.Lorem.Sentences()),
            _identityServiceMock.Object.GetCurrent());
        aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()), _identityServiceMock.Object.GetCurrent());
        aggregate.ExtendExpirationDate(Date.Create(_faker.Date.FutureOffset()),
            _identityServiceMock.Object.GetCurrent());

        _aggregateStoreMock.Setup(x => x.Exists(aggregateIdentifier.Value, expectedAggregateVersion)).Returns(true);

        IEventSourcingAggregateRepository<Post, PostIdentifier> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object);

        // Act
        Action saveAction = () => eventSourcingAggregateRepository.Save(aggregate, expectedAggregateVersion);

        // Assert
        saveAction.Should().Throw<ConcurrencyException>().WithMessage(
            $"A concurrency violation occurred on this aggregate ({aggregateIdentifier}). At least one event failed to save");

        _aggregateStoreMock.Verify(x => x.Exists(aggregateIdentifier.Value, expectedAggregateVersion), Times.Once);
    }

    [Test]
    public async Task SaveAsync_WhenCorrectAggregateProvided_ShouldSaveAggregate()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null, _identityServiceMock.Object.GetCurrent());

        aggregate.ChangeDescription(Description.Create(_faker.Lorem.Sentences()),
            _identityServiceMock.Object.GetCurrent());
        aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()), _identityServiceMock.Object.GetCurrent());
        aggregate.ExtendExpirationDate(Date.Create(_faker.Date.FutureOffset()),
            _identityServiceMock.Object.GetCurrent());

        EventSourcingAggregateDataModel eventSourcingAggregateDataModel = new()
        {
            AggregateIdentifier = aggregate.AggregateIdentifier.Value,
            AggregateVersion = aggregate.AggregateVersion,
            DeletedDate = null,
            IsDeleted = false,
            TenantIdentifier = _identityServiceMock.Object.GetCurrent().Tenant.Identifier
        };

        IDomainEvent<PostIdentifier>[] events = aggregate.GetUncommittedChanges();
        List<DomainEventDataModel> eventDataModels =
            GenerateDomainEventDataModelsCollection(aggregateIdentifier, events);

        _aggregateMapperMock.Setup(x => x.Map(aggregate)).Returns(eventSourcingAggregateDataModel);
        _domainEventMapperMock.Setup(x => x.Map(events)).Returns(eventDataModels);
        _aggregateStoreMock.Setup(x => x.SaveAsync(eventSourcingAggregateDataModel, eventDataModels, default));

        IEventSourcingAggregateRepository<Post, PostIdentifier> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object);

        // Act
        IDomainEvent<PostIdentifier>[] domainEvents = await eventSourcingAggregateRepository.SaveAsync(aggregate);

        // Assert
        domainEvents.Should().BeEquivalentTo(events);
        _aggregateMapperMock.Verify(x => x.Map(aggregate), Times.Once());
        _domainEventMapperMock.Verify(x => x.Map(events), Times.Once());
        _aggregateStoreMock.Verify(x => x.SaveAsync(eventSourcingAggregateDataModel, eventDataModels, default),
            Times.Once());
    }

    [Test]
    public async Task SaveAsync_WhenNextVersionIsOccupied_ShouldThrowConcurrencyException()
    {
        // Arrange
        PostIdentifier aggregateIdentifier = PostIdentifier.New(_faker.Random.Guid());
        const int aggregateVersion = 0;
        const int expectedAggregateVersion = 0;

        Post aggregate =
            EventSourcingGenericAggregateFactory<Post, PostIdentifier>.CreateAggregate(aggregateIdentifier,
                aggregateVersion);

        aggregate.Create(Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
            Date.Create(_faker.Date.FutureOffset()), null, _identityServiceMock.Object.GetCurrent());

        aggregate.ChangeDescription(Description.Create(_faker.Lorem.Sentences()),
            _identityServiceMock.Object.GetCurrent());
        aggregate.ChangeTitle(Title.Default(_faker.Lorem.Sentence()), _identityServiceMock.Object.GetCurrent());
        aggregate.ExtendExpirationDate(Date.Create(_faker.Date.FutureOffset()),
            _identityServiceMock.Object.GetCurrent());

        _aggregateStoreMock.Setup(x => x.ExistsAsync(aggregateIdentifier.Value, expectedAggregateVersion, default))
            .ReturnsAsync(true);

        IEventSourcingAggregateRepository<Post, PostIdentifier> eventSourcingAggregateRepository =
            new EventSourcingAggregateRepository<Post, PostIdentifier>(_aggregateStoreMock.Object,
                _domainEventMapperMock.Object, _aggregateMapperMock.Object);

        // Act
        Func<Task> saveAction = async () =>
            await eventSourcingAggregateRepository.SaveAsync(aggregate, expectedAggregateVersion);

        // Assert
        await saveAction.Should().ThrowAsync<ConcurrencyException>().WithMessage(
            $"A concurrency violation occurred on this aggregate ({aggregateIdentifier}). At least one event failed to save");

        _aggregateStoreMock.Verify(x => x.ExistsAsync(aggregateIdentifier.Value, expectedAggregateVersion, default),
            Times.Once);
    }

    private List<IDomainEvent<PostIdentifier>> GenerateDomainEventsCollection(PostIdentifier aggregateIdentifier)
    {
        Guid imageId = _faker.Random.Guid();

        List<IDomainEvent<PostIdentifier>> domainEvents = new()
        {
            new CreateNewPostDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 1,
                Title.Default(_faker.Lorem.Sentence()), Description.Create(_faker.Lorem.Sentences()),
                Date.Create(_faker.Date.FutureOffset()), null),
            new ChangeDescriptionDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 2,
                Description.Create(_faker.Lorem.Sentences())),
            new AddImagesDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 3,
                new HashSet<Image>
                {
                    Image.Create(imageId, Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                        BytesContent.Create(_faker.Random.Bytes(2346))),
                    Image.Create(_faker.Random.Guid(),
                        Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150))),
                        BytesContent.Create(_faker.Random.Bytes(982)))
                }),
            new ChangeImageNameDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 4,
                imageId, Name.Create(_faker.Random.AlphaNumeric(_faker.Random.Number(3, 150)))),
            new ChangeExpirationDateDomainEvent(aggregateIdentifier, _identityServiceMock.Object.GetCurrent(), 5,
                Date.Create(_faker.Date.Future()))
        };

        return domainEvents;
    }

    private List<DomainEventDataModel> GenerateDomainEventDataModelsCollection(PostIdentifier aggregateIdentifier,
        IEnumerable<IDomainEvent<PostIdentifier>> events = null)
    {
        IDomainEventMapper<PostIdentifier> domainEventMapper = new DomainEventMapper<PostIdentifier>();
        List<IDomainEvent<PostIdentifier>> domainEvents =
            events?.ToList() ?? GenerateDomainEventsCollection(aggregateIdentifier);
        List<DomainEventDataModel> domainEventDms = domainEventMapper.Map(domainEvents).ToList();

        return domainEventDms;
    }
}