using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Exceptions;
using EssentialFrame.Cqrs.Commands.Persistence.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence.Models;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Persistence;

internal sealed class CommandMapper : ICommandMapper
{
    private readonly ICommandDataModelService _commandDataModelService;

    public CommandMapper(ICommandDataModelService commandDataModelService)
    {
        _commandDataModelService =
            commandDataModelService ?? throw new ArgumentNullException(nameof(commandDataModelService));
    }

    public CommandDataModel Map(ICommand command)
    {
        return _commandDataModelService.Create(command);
    }

    public CommandDataModel Map(ICommand command, ISerializer serializer)
    {
        return _commandDataModelService.Create(command, serializer);
    }

    public IReadOnlyCollection<CommandDataModel> Map(IEnumerable<ICommand> commands)
    {
        return commands.Select(Map).ToList();
    }

    public IReadOnlyCollection<CommandDataModel> Map(IEnumerable<ICommand> commands, ISerializer serializer)
    {
        return commands.Select(c => Map(c, serializer)).ToList();
    }

    public ICommand Map(CommandDataModel commandDataModel)
    {
        return commandDataModel.Command as ICommand;
    }

    public ICommand Map(CommandDataModel commandDataModel, ISerializer serializer)
    {
        string serializedCommand = commandDataModel.Command as string;

        ICommand deserialized =
            serializer.Deserialize<ICommand>(serializedCommand, Type.GetType(commandDataModel.CommandClass));

        if (deserialized is null)
        {
            throw new UnknownCommandTypeException(commandDataModel.CommandType);
        }

        return deserialized;
    }

    public IReadOnlyCollection<ICommand> Map(IEnumerable<CommandDataModel> commandDataModels)
    {
        return commandDataModels.Select(Map).ToList();
    }

    public IReadOnlyCollection<ICommand> Map(IEnumerable<CommandDataModel> commandDataModels, ISerializer serializer)
    {
        return commandDataModels.Select(c => Map(c, serializer)).ToList();
    }
}