using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using EssentialFrame.Cache.Interfaces;
using EssentialFrame.Cqrs.Commands.Store;
using EssentialFrame.Cqrs.Commands.Store.Models;
using EssentialFrame.Identity;
using EssentialFrame.TestData.Commands;
using EssentialFrame.TestData.Identity;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Cqrs.Commands.UnitTests.Commands;

[TestFixture]
public class DefaultCommandStoreTests
{
    private readonly Faker _faker = new();
    private Mock<ICache<Guid, CommandDao>> _commandsCacheMock;
    private Mock<IIdentityService> _identityServiceMock;

    [SetUp]
    public void SetUp()
    {
        _commandsCacheMock = new Mock<ICache<Guid, CommandDao>>();
        _identityServiceMock = new Mock<IIdentityService>();

        _identityServiceMock.Setup(x => x.GetCurrent()).Returns(new TestIdentity());
    }

    [TearDown]
    public void TearDown()
    {
        _commandsCacheMock.Reset();
        _identityServiceMock.Reset();
    }

    [Test]
    public void Exists_WhenCommandIdentifierIsProvided_ShouldReturnTrue()
    {
        // Arrange
        Guid commandIdentifier = _faker.Random.Guid();
        _commandsCacheMock.Setup(x => x.Exists(commandIdentifier)).Returns(true);
        DefaultCommandStore commandStore = new DefaultCommandStore(_commandsCacheMock.Object);

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
        DefaultCommandStore commandStore = new DefaultCommandStore(_commandsCacheMock.Object);

        // Act
        bool result = await commandStore.ExistsAsync(commandIdentifier);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void Get_WhenCommandIdentifierIsProvided_ShouldReturnCommandDao()
    {
        // Arrange
        Guid commandIds = _faker.Random.Guid();
        CommandDao commandDao = new CommandDao(new ChangeTitleCommand(_identityServiceMock.Object.GetCurrent(),
            _faker.Lorem.Word(), _faker.Random.Bool()));

        _commandsCacheMock.Setup(x => x[commandIds]).Returns(commandDao);
        DefaultCommandStore commandStore = new DefaultCommandStore(_commandsCacheMock.Object);

        // Act
        CommandDao result = commandStore.Get(commandIds);

        // Assert
        result.Should().BeEquivalentTo(commandDao);
        _identityServiceMock.VerifyAll();
    }

    [Test]
    public async Task GetAsync_WhenCommandIdentifierIsProvided_ShouldReturnCommandDao()
    {
        // Arrange
        Guid commandIds = _faker.Random.Guid();
        CommandDao commandDao = new CommandDao(new ChangeTitleCommand(_identityServiceMock.Object.GetCurrent(),
            _faker.Lorem.Word(), _faker.Random.Bool()));

        _commandsCacheMock.Setup(x => x[commandIds]).Returns(commandDao);
        DefaultCommandStore commandStore = new DefaultCommandStore(_commandsCacheMock.Object);

        // Act
        CommandDao result = await commandStore.GetAsync(commandIds);

        // Assert
        result.Should().BeEquivalentTo(commandDao);
        _identityServiceMock.VerifyAll();
    }

    [Test]
    public void GetPossibleToSend_WhenDateTimeOffsetIsProvided_ShouldReturnCommandDao()
    {
        // Arrange
        CommandDao commandDao = new CommandDao(new ChangeTitleCommand(_identityServiceMock.Object.GetCurrent(),
            _faker.Lorem.Word(), _faker.Random.Bool()));

        CommandDao[] commandDaoArray = new[] { commandDao };
        _commandsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, CommandDao, bool>>())).Returns(commandDaoArray);
        DefaultCommandStore commandStore = new DefaultCommandStore(_commandsCacheMock.Object);

        // Act
        IReadOnlyCollection<CommandDao> result = commandStore.GetPossibleToSend(_faker.Date.RecentOffset());

        // Assert
        result.Should().BeEquivalentTo(commandDaoArray);
    }

    [Test]
    public async Task GetPossibleToSendAsync_WhenDateTimeOffsetIsProvided_ShouldReturnCommandDao()
    {
        // Arrange
        CommandDao commandDao = new CommandDao(new ChangeTitleCommand(_identityServiceMock.Object.GetCurrent(),
            _faker.Lorem.Word(), _faker.Random.Bool()));

        CommandDao[] commandDaoArray = new[] { commandDao };
        _commandsCacheMock.Setup(x => x.GetMany(It.IsAny<Func<Guid, CommandDao, bool>>())).Returns(commandDaoArray);
        DefaultCommandStore commandStore = new DefaultCommandStore(_commandsCacheMock.Object);

        // Act
        IReadOnlyCollection<CommandDao> result = await commandStore.GetPossibleToSendAsync(_faker.Date.RecentOffset());

        // Assert
        result.Should().BeEquivalentTo(commandDaoArray);
    }

    [Test]
    public void Save_WhenCommandDaoIsProvided_ShouldSaveCommandDao()
    {
        // Arrange
        bool isNew = _faker.Random.Bool();

        CommandDao commandDao = new CommandDao(new ChangeTitleCommand(_identityServiceMock.Object.GetCurrent(),
            _faker.Lorem.Word(), _faker.Random.Bool()));

        _commandsCacheMock.Setup(x => x.Add(commandDao.CommandIdentifier, commandDao));
        DefaultCommandStore commandStore = new DefaultCommandStore(_commandsCacheMock.Object);

        // Act
        commandStore.Save(commandDao, isNew);

        // Assert
        _commandsCacheMock.Verify(x => x.Add(commandDao.CommandIdentifier, commandDao), Times.Once);
        _identityServiceMock.VerifyAll();
    }

    [Test]
    public async Task SaveAsync_WhenCommandDaoIsProvided_ShouldSaveCommandDao()
    {
        // Arrange
        bool isNew = _faker.Random.Bool();

        CommandDao commandDao = new CommandDao(new ChangeTitleCommand(_identityServiceMock.Object.GetCurrent(),
            _faker.Lorem.Word(), _faker.Random.Bool()));

        _commandsCacheMock.Setup(x => x.Add(commandDao.CommandIdentifier, commandDao));
        DefaultCommandStore commandStore = new DefaultCommandStore(_commandsCacheMock.Object);

        // Act
        await commandStore.SaveAsync(commandDao, isNew);

        // Assert
        _commandsCacheMock.Verify(x => x.Add(commandDao.CommandIdentifier, commandDao), Times.Once);
        _identityServiceMock.VerifyAll();
    }
}