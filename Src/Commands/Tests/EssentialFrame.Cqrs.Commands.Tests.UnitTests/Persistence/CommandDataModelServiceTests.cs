using System;
using Bogus;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence;
using EssentialFrame.Cqrs.Commands.Persistence.Const;
using EssentialFrame.Cqrs.Commands.Persistence.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence.Models;
using EssentialFrame.ExampleApp.Commands.Posts;
using EssentialFrame.ExampleApp.Identity;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.Tests.Utils;
using EssentialFrame.Time;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Cqrs.Commands.Tests.UnitTests.Persistence;

[TestFixture]
public class CommandDataModelServiceTests
{
    private readonly Faker _faker = new();
    private readonly Mock<ISerializer> _serializerMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private CommandDataModel _commandDataModel;

    [SetUp]
    public void Setup()
    {
        _commandDataModel = new CommandDataModel
        {
            CommandIdentifier = _faker.Random.Guid(),
            CommandClass = typeof(ChangeTitleCommand).AssemblyQualifiedName,
            CommandType = typeof(ChangeTitleCommand).FullName,
            Command = _faker.Random.String(150, 300),
            CreatedAt = SystemClock.UtcNow
        };

        _identityServiceMock.Setup(x => x.GetCurrent()).Returns(new IdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _commandDataModel = null;
        _serializerMock.Reset();
        _identityServiceMock.Reset();
    }

    [Test]
    public void Create_WhenCommandIsProvided_ShouldReturnCommandDataModel()
    {
        // Arrange
        ChangeTitleCommand command = new(_identityServiceMock.Object.GetCurrent(), _faker.Random.String(10, 20),
            _faker.Random.Bool());

        ICommandDataModelService commandDataModelService = new CommandDataModelService();

        // Act
        CommandDataModel result = commandDataModelService.Create(command);

        // Assert
        result.Should().NotBeNull();
        result.CommandIdentifier.Should().NotBeEmpty();
        result.CommandClass.Should().NotBeEmpty();
        result.CommandType.Should().NotBeEmpty();
        result.Command.Should().BeEquivalentTo(command);
        result.CreatedAt.Should().BeCloseTo(SystemClock.UtcNow, TimeSpan.FromMilliseconds(Defaults.DefaultCloseTo));
    }

    [Test]
    public void Create_WhenProvidedCommandIsNull_ShouldThrowArgumentException()
    {
        // Arrange
        ICommandDataModelService commandDataModelService = new CommandDataModelService();

        // Act
        Action act = () => commandDataModelService.Create(null);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Command class cannot be null.");
    }

    [Test]
    public void Create_WhenCommandAndSerializerAreProvided_ShouldReturnCommandDataModel()
    {
        // Arrange
        ChangeTitleCommand command = new(_identityServiceMock.Object.GetCurrent(), _faker.Random.String(10, 20),
            _faker.Random.Bool());

        string serializedCommand = _faker.Random.String(10, 35);
        _serializerMock.Setup(x => x.Serialize(It.IsAny<ICommand>())).Returns(serializedCommand);

        ICommandDataModelService commandDataModelService = new CommandDataModelService();

        // Act
        CommandDataModel result = commandDataModelService.Create(command, _serializerMock.Object);

        // Assert
        _serializerMock.Verify(x => x.Serialize(It.IsAny<ICommand>()), Times.Once);

        result.Should().NotBeNull();
        result.CommandIdentifier.Should().NotBeEmpty();
        result.CommandClass.Should().NotBeEmpty();
        result.CommandType.Should().NotBeEmpty();
        result.Command.Should().Be(serializedCommand);
        result.CreatedAt.Should().BeCloseTo(SystemClock.UtcNow, TimeSpan.FromMilliseconds(Defaults.DefaultCloseTo));
    }

    [Test]
    public void Create_WhenSerializerIsProvidedButCommandIsNull_ShouldThrowArgumentException()
    {
        // Arrange
        ICommandDataModelService commandDataModelService = new CommandDataModelService();

        // Act
        Action act = () => commandDataModelService.Create(null, _serializerMock.Object);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Command class cannot be null.");
    }

    [Test]
    public void Start_WhenCommandDataModelIsProvided_ShouldReturnCommandDataModel()
    {
        // Arrange
        ICommandDataModelService commandDataModelService = new CommandDataModelService();

        // Act
        commandDataModelService.Start(_commandDataModel);

        // Assert
        _commandDataModel.SendStarted.Should()
            .BeCloseTo(SystemClock.UtcNow, TimeSpan.FromMilliseconds(Defaults.DefaultCloseTo));
        _commandDataModel.SendStatus.Should().Be(CommandSendingStatuses.Started);
        _commandDataModel.ExecutionStatus.Should().Be(CommandExecutionStatuses.WaitingForExecution);
    }

    [Test]
    public void Schedule_WhenCommandDataModelIsProvided_ShouldReturnCommandDataModel()
    {
        // Arrange
        DateTimeOffset at = _faker.Date.FutureOffset();
        ICommandDataModelService commandDataModelService = new CommandDataModelService();

        // Act
        commandDataModelService.Schedule(_commandDataModel, at);

        // Assert
        _commandDataModel.SendScheduled.Should().BeCloseTo(at, TimeSpan.FromMilliseconds(Defaults.DefaultCloseTo));
        _commandDataModel.SendStatus.Should().Be(CommandSendingStatuses.Scheduled);
        _commandDataModel.ExecutionStatus.Should().Be(CommandExecutionStatuses.WaitingForExecution);
    }

    [Test]
    public void Cancel_WhenCommandDataModelIsProvided_ShouldReturnCommandDataModel()
    {
        // Arrange
        ICommandDataModelService commandDataModelService = new CommandDataModelService();

        // Act
        commandDataModelService.Cancel(_commandDataModel);

        // Assert
        _commandDataModel.SendCancelled.Should()
            .BeCloseTo(SystemClock.UtcNow, TimeSpan.FromMilliseconds(Defaults.DefaultCloseTo));
        _commandDataModel.SendStatus.Should().Be(CommandSendingStatuses.Cancelled);
        _commandDataModel.ExecutionStatus.Should().Be(CommandExecutionStatuses.ExecutionCancelled);
    }

    [Test]
    public void Complete_WhenCommandDataModelIsProvidedAndExecutionSucceed_ShouldReturnCommandDataModel()
    {
        // Arrange
        ICommandDataModelService commandDataModelService = new CommandDataModelService();

        // Act
        commandDataModelService.Complete(_commandDataModel, true);

        // Assert
        _commandDataModel.SendCompleted.Should()
            .BeCloseTo(SystemClock.UtcNow, TimeSpan.FromMilliseconds(Defaults.DefaultCloseTo));
        _commandDataModel.SendStatus.Should().Be(CommandSendingStatuses.Completed);
        _commandDataModel.ExecutionStatus.Should().Be(CommandExecutionStatuses.ExecutedSuccessfully);
    }

    [Test]
    public void Complete_WhenCommandDataModelIsProvidedAndExecutionFailed_ShouldReturnCommandDataModel()
    {
        // Arrange
        ICommandDataModelService commandDataModelService = new CommandDataModelService();

        // Act
        commandDataModelService.Complete(_commandDataModel, false);

        // Assert
        _commandDataModel.SendCompleted.Should()
            .BeCloseTo(SystemClock.UtcNow, TimeSpan.FromMilliseconds(Defaults.DefaultCloseTo));
        _commandDataModel.SendStatus.Should().Be(CommandSendingStatuses.Completed);
        _commandDataModel.ExecutionStatus.Should().Be(CommandExecutionStatuses.ExecutedWithErrors);
    }
}