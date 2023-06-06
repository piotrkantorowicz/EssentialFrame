using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence;
using EssentialFrame.Cqrs.Commands.Persistence.Models;
using EssentialFrame.ExampleApp.Commands.Posts;
using EssentialFrame.ExampleApp.Identity;
using EssentialFrame.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Cqrs.Commands.Tests.UnitTests.Store;

[TestFixture]
public class DefaultCommandStoreTests
{
    [SetUp]
    public void SetUp()
    {
        _commandsCacheMock = new Mock<ICache<Guid, CommandDataModel>>();
        _identityServiceMock = new Mock<IIdentityService>();

        _identityServiceMock.Setup(x => x.GetCurrent()).Returns(new IdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _commandsCacheMock.Reset();
        _identityServiceMock.Reset();
    }

    private readonly Faker _faker = new();
    private Mock<ICache<Guid, CommandDataModel>> _commandsCacheMock;
    private Mock<IIdentityService> _identityServiceMock;

    [Test]
    public void Exists_WhenCommandIdentifierIsProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid commandIdentifier = _faker.Random.Guid();
        _commandsCacheMock.Setup(x => x.Exists(commandIdentifier)).Returns(true);
        DefaultCommandStore commandStore = new(_commandsCacheMock.Object);

        // Act
        bool result = commandStore.Exists(commandIdentifier);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task ExistsAsync_WhenCommandIdentifierIsProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid commandIdentifier = Guid.NewGuid();
        _commandsCacheMock.Setup(x => x.Exists(commandIdentifier)).Returns(true);
        DefaultCommandStore commandStore = new(_commandsCacheMock.Object);

        // Act
        bool result = await commandStore.ExistsAsync(commandIdentifier);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Get_WhenCommandIdentifierIsProvided_ShouldReturnCommandDataModel()
    {
        // Arrange
        Guid commandIds = _faker.Random.Guid();
        CommandDataModel commandDataModel = new(new ChangeTitleCommand(_identityServiceMock.Object.GetCurrent(),
            _faker.Lorem.Word(), _faker.Random.Bool()));

        _commandsCacheMock.Setup(x => x[commandIds]).Returns(commandDataModel);
        DefaultCommandStore commandStore = new(_commandsCacheMock.Object);

        // Act
        CommandDataModel result = commandStore.Get(commandIds);

        // Assert
        result.Should().BeEquivalentTo(commandDataModel);
        _identityServiceMock.VerifyAll();
    }

    [Test]
    public async Task GetAsync_WhenCommandIdentifierIsProvided_ShouldReturnCommandDataModel()
    {
        // Arrange
        Guid commandIds = _faker.Random.Guid();
        CommandDataModel commandDataModel = new(new ChangeTitleCommand(_identityServiceMock.Object.GetCurrent(),
            _faker.Lorem.Word(), _faker.Random.Bool()));

        _commandsCacheMock.Setup(x => x[commandIds]).Returns(commandDataModel);
        DefaultCommandStore commandStore = new(_commandsCacheMock.Object);

        // Act
        CommandDataModel result = await commandStore.GetAsync(commandIds);

        // Assert
        result.Should().BeEquivalentTo(commandDataModel);
        _identityServiceMock.VerifyAll();
    }

    [Test]
    public void GetPossibleToSend_WhenDateTimeOffsetIsProvided_ShouldReturnCommandDataModel()
    {
        // Arrange
        CommandDataModel commandDataModel = new(new ChangeTitleCommand(_identityServiceMock.Object.GetCurrent(),
            _faker.Lorem.Word(), _faker.Random.Bool()));

        CommandDataModel[] commandDataModelsArray = { commandDataModel };
        _commandsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, CommandDataModel, bool>>()))
            .Returns(commandDataModelsArray);
        DefaultCommandStore commandStore = new(_commandsCacheMock.Object);

        // Act
        IReadOnlyCollection<CommandDataModel> result = commandStore.GetPossibleToSend(_faker.Date.RecentOffset());

        // Assert
        result.Should().BeEquivalentTo(commandDataModelsArray);
    }

    [Test]
    public async Task GetPossibleToSendAsync_WhenDateTimeOffsetIsProvided_ShouldReturnCommandDataModel()
    {
        // Arrange
        CommandDataModel commandDataModel = new(new ChangeTitleCommand(_identityServiceMock.Object.GetCurrent(),
            _faker.Lorem.Word(), _faker.Random.Bool()));

        CommandDataModel[] commandDataModelsArray = { commandDataModel };
        _commandsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, CommandDataModel, bool>>()))
            .Returns(commandDataModelsArray);
        DefaultCommandStore commandStore = new(_commandsCacheMock.Object);

        // Act
        IReadOnlyCollection<CommandDataModel> result =
            await commandStore.GetPossibleToSendAsync(_faker.Date.RecentOffset());

        // Assert
        result.Should().BeEquivalentTo(commandDataModelsArray);
    }

    [Test]
    public void Save_WhenCommandDataModelIsProvided_ShouldSaveCommandDataModel()
    {
        // Arrange
        bool isNew = _faker.Random.Bool();

        CommandDataModel commandDataModel = new(new ChangeTitleCommand(_identityServiceMock.Object.GetCurrent(),
            _faker.Lorem.Word(), _faker.Random.Bool()));

        _commandsCacheMock.Setup(x => x.Add(commandDataModel.CommandIdentifier, commandDataModel));
        DefaultCommandStore commandStore = new(_commandsCacheMock.Object);

        // Act
        commandStore.Save(commandDataModel, isNew);

        // Assert
        _commandsCacheMock.Verify(x => x.Add(commandDataModel.CommandIdentifier, commandDataModel), Times.Once);
        _identityServiceMock.VerifyAll();
    }

    [Test]
    public async Task SaveAsync_WhenCommandDataModelIsProvided_ShouldSaveCommandDataModel()
    {
        // Arrange
        bool isNew = _faker.Random.Bool();

        CommandDataModel commandDataModel = new(new ChangeTitleCommand(_identityServiceMock.Object.GetCurrent(),
            _faker.Lorem.Word(), _faker.Random.Bool()));

        _commandsCacheMock.Setup(x => x.Add(commandDataModel.CommandIdentifier, commandDataModel));
        DefaultCommandStore commandStore = new(_commandsCacheMock.Object);

        // Act
        await commandStore.SaveAsync(commandDataModel, isNew);

        // Assert
        _commandsCacheMock.Verify(x => x.Add(commandDataModel.CommandIdentifier, commandDataModel), Times.Once);
        _identityServiceMock.VerifyAll();
    }
}