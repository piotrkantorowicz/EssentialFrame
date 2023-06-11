using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence;
using EssentialFrame.Cqrs.Commands.Persistence.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence.Models;
using EssentialFrame.ExampleApp.Commands.Posts;
using EssentialFrame.ExampleApp.Identity;
using EssentialFrame.Extensions;
using EssentialFrame.Identity;
using EssentialFrame.Serialization.Interfaces;
using EssentialFrame.Tests.Utils;
using EssentialFrame.Time;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace EssentialFrame.Cqrs.Commands.Tests.UnitTests.Persistence;

[TestFixture]
public class CommandMapperTests
{
    private readonly Faker _faker = new();
    private readonly Mock<ISerializer> _serializerMock = new();
    private readonly Mock<ICommandDataModelService> _commandDataModelServiceMock = new();
    private readonly Mock<IIdentityService> _identityServiceMock = new();

    [SetUp]
    public void Setup()
    {
        _identityServiceMock.Setup(ism => ism.GetCurrent()).Returns(new IdentityContext());
    }

    [TearDown]
    public void TearDown()
    {
        _serializerMock.Reset();
        _commandDataModelServiceMock.Reset();
        _identityServiceMock.Reset();
    }

    [Test]
    public void Map_WhenCommandIsNotSerialized_ShouldMapCommand()
    {
        // Arrange
        string title = _faker.Lorem.Sentence();
        bool isUppercase = _faker.Random.Bool();
        ChangeTitleCommand command = new(_identityServiceMock.Object.GetCurrent(), title, isUppercase);
        CommandDataModel commandDataModel = GenerateCommandDataModel(command);

        _commandDataModelServiceMock.Setup(x => x.Create(command)).Returns(commandDataModel);

        ICommandMapper commandMapper = new CommandMapper(_commandDataModelServiceMock.Object);

        // Act
        CommandDataModel result = commandMapper.Map(command);

        // Assert
        _commandDataModelServiceMock.Verify(x => x.Create(command), Times.Once);

        AssertCommand(result, command);
    }

    [Test]
    public void Map_WhenCommandIsSerialized_ShouldMapCommand()
    {
        // Arrange
        string title = _faker.Lorem.Sentence();
        bool isUppercase = _faker.Random.Bool();
        ChangeTitleCommand command = new(_identityServiceMock.Object.GetCurrent(), title, isUppercase);
        CommandDataModel commandDataModel = GenerateCommandDataModel(command);

        _commandDataModelServiceMock.Setup(x => x.Create(command, _serializerMock.Object)).Returns(commandDataModel);

        ICommandMapper commandMapper = new CommandMapper(_commandDataModelServiceMock.Object);

        // Act
        CommandDataModel result = commandMapper.Map(command, _serializerMock.Object);

        // Assert
        _commandDataModelServiceMock.Verify(x => x.Create(command, _serializerMock.Object), Times.Once);

        AssertCommand(result, command);
    }

    [Test]
    public void Map_WhenCommandIsNotSerialized_ShouldMapCollectionOfCommands()
    {
        // Arrange
        IReadOnlyCollection<ICommand> commands = GenerateCommands();
        ICommandMapper commandMapper = new CommandMapper(_commandDataModelServiceMock.Object);

        foreach (ICommand command in commands)
        {
            CommandDataModel commandDataModel = GenerateCommandDataModel(command);
            _commandDataModelServiceMock.Setup(x => x.Create(command)).Returns(commandDataModel);
        }
        
        // Act
        IEnumerable<CommandDataModel> result = commandMapper.Map(commands);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(commands.Count);

        foreach (CommandDataModel commandDataModel in result)
        {
            ICommand command = commands.Single(c => c.CommandIdentifier == commandDataModel.CommandIdentifier);

            _commandDataModelServiceMock.Setup(x => x.Create(command)).Returns(commandDataModel);

            AssertCommand(commandDataModel, command);
        }
    }

    [Test]
    public void Map_WhenCommandIsSerialized_ShouldMapCollectionOfCommands()
    {
        // Arrange
        IReadOnlyCollection<ICommand> commands = GenerateCommands();
        ICommandMapper commandMapper = new CommandMapper(_commandDataModelServiceMock.Object);

        foreach (ICommand command in commands)
        {
            CommandDataModel commandDataModel = GenerateCommandDataModel(command);
            _commandDataModelServiceMock.Setup(x => x.Create(command, _serializerMock.Object))
                .Returns(commandDataModel);
        }

        // Act
        IEnumerable<CommandDataModel> result = commandMapper.Map(commands, _serializerMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(commands.Count);

        foreach (CommandDataModel commandDataModel in result)
        {
            ICommand command = commands.Single(c => c.CommandIdentifier == commandDataModel.CommandIdentifier);

            _commandDataModelServiceMock.Setup(x => x.Create(command, _serializerMock.Object))
                .Returns(commandDataModel);

            AssertCommand(commandDataModel, command);
        }
    }

    [Test]
    public void Map_WhenCommandDataIsNotSerialized_ShouldMapCommandDataModel()
    {
        // Arrange
        string title = _faker.Lorem.Sentence();
        bool isUppercase = _faker.Random.Bool();
        ChangeTitleCommand command = new(_identityServiceMock.Object.GetCurrent(), title, isUppercase);
        CommandDataModel commandDataModel = GenerateCommandDataModel(command);
        ICommandMapper commandMapper = new CommandMapper(_commandDataModelServiceMock.Object);

        // Act
        ICommand result = commandMapper.Map(commandDataModel);

        // Assert
        AssertCommand(commandDataModel, result);
    }

    [Test]
    public void Map_WhenCommandDataIsSerialized_ShouldMapCommandDataModel()
    {
        // Arrange
        string title = _faker.Lorem.Sentence();
        bool isUppercase = _faker.Random.Bool();
        string serializedCommand = _faker.Lorem.Sentences();
        ChangeTitleCommand command = new(_identityServiceMock.Object.GetCurrent(), title, isUppercase);

        _serializerMock.Setup(s => s.Serialize<ICommand>(command)).Returns(serializedCommand);

        CommandDataModel commandDataModel = GenerateCommandDataModel(command, _serializerMock.Object);
        ICommandMapper commandMapper = new CommandMapper(_commandDataModelServiceMock.Object);

        _serializerMock
            .Setup(s => s.Deserialize<ICommand>(commandDataModel.Command as string, typeof(ChangeTitleCommand)))
            .Returns(command);

        // Act
        ICommand result = commandMapper.Map(commandDataModel, _serializerMock.Object);

        // Assert
        _serializerMock.Verify(s => s.Serialize<ICommand>(command), Times.Once);
        _serializerMock.Verify(
            s => s.Deserialize<ICommand>(commandDataModel.Command as string, typeof(ChangeTitleCommand)), Times.Once);

        AssertCommand(commandDataModel, result, _serializerMock.Object);
    }

    [Test]
    public void Map_WhenCommandDataIsNotSerialized_ShouldMapCollectionOfCommandDataModels()
    {
        // Arrange
        IReadOnlyCollection<ICommand> commands = GenerateCommands();
        List<CommandDataModel> commandDataModels =
            commands.Select(command => GenerateCommandDataModel(command)).ToList();
        ICommandMapper commandMapper = new CommandMapper(_commandDataModelServiceMock.Object);

        // Act
        IEnumerable<ICommand> result = commandMapper.Map(commandDataModels);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(commandDataModels.Count);

        foreach (ICommand command in result)
        {
            AssertCommand(commandDataModels.Single(c => c.CommandIdentifier == command.CommandIdentifier), command);
        }
    }

    [Test]
    public void Map_WhenCommandDataIsSerialized_ShouldMapCollectionOfCommandDataModels()
    {
        // Arrange
        IReadOnlyCollection<ICommand> commands = GenerateCommands();
        List<CommandDataModel> commandDataModels = new();

        foreach (ICommand command in commands)
        {
            _serializerMock.Setup(s => s.Serialize(command)).Returns(_faker.Lorem.Sentences());

            CommandDataModel commandDataModel = GenerateCommandDataModel(command, _serializerMock.Object);
            commandDataModels.Add(commandDataModel);

            _serializerMock
                .Setup(s => s.Deserialize<ICommand>(commandDataModel.Command as string, typeof(ChangeTitleCommand)))
                .Returns(command);
        }

        ICommandMapper commandMapper = new CommandMapper(_commandDataModelServiceMock.Object);

        // Act
        IEnumerable<ICommand> result = commandMapper.Map(commandDataModels, _serializerMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(commandDataModels.Count);

        foreach (ICommand command in result)
        {
            CommandDataModel commandDataModel =
                commandDataModels.Single(c => c.CommandIdentifier == command.CommandIdentifier);

            _serializerMock.Verify(s => s.Serialize(command), Times.Once);
            _serializerMock.Verify(
                s => s.Deserialize<ICommand>(commandDataModel.Command as string, typeof(ChangeTitleCommand)),
                Times.Once);

            AssertCommand(commandDataModel, command, _serializerMock.Object);
        }
    }

    private CommandDataModel GenerateCommandDataModel(ICommand command, ISerializer serializer = null)
    {
        CommandDataModel commandDataModel = new()
        {
            CommandIdentifier = command.CommandIdentifier,
            CommandClass = command.GetClassName(),
            CommandType = command.GetTypeFullName(),
            Command = serializer is null ? command : serializer.Serialize(command),
            CreatedAt = SystemClock.UtcNow
        };

        return commandDataModel;
    }

    private IReadOnlyCollection<ICommand> GenerateCommands()
    {
        List<ICommand> commands = new();
        string title = _faker.Lorem.Sentence();
        bool isUppercase = _faker.Random.Bool();
        IIdentityContext identityContext = _identityServiceMock.Object.GetCurrent();

        for (int i = 0; i < _faker.Random.Int(1, 20); i++)
        {
            commands.Add(new ChangeTitleCommand(identityContext, title, isUppercase));
        }

        return commands;
    }

    private static void AssertCommand(CommandDataModel commandDataModel, ICommand command,
        ISerializer serializer = null)
    {
        commandDataModel.Should().NotBeNull();
        commandDataModel.CommandIdentifier.Should().Be(command.CommandIdentifier);
        commandDataModel.CommandType.Should().Be(command.GetType().FullName);
        commandDataModel.CommandClass.Should().Be(command.GetType().AssemblyQualifiedName);

        if (serializer is null)
        {
            commandDataModel.Command.Should().BeEquivalentTo(command);
        }
        else
        {
            commandDataModel.Command.Should().BeEquivalentTo(serializer.Serialize(command));
        }

        commandDataModel.SendScheduled.Should().BeNull();
        commandDataModel.SendStarted.Should().BeNull();
        commandDataModel.SendCompleted.Should().BeNull();
        commandDataModel.SendCancelled.Should().BeNull();
        commandDataModel.SendStatus.Should().BeNull();
        commandDataModel.ExecutionStatus.Should().BeNull();
        commandDataModel.CreatedAt.Should()
            .BeCloseTo(SystemClock.UtcNow, TimeSpan.FromMilliseconds(Defaults.DefaultCloseTo));
    }
}