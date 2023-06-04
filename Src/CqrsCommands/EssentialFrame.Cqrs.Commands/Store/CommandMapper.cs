using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Exceptions;
using EssentialFrame.Cqrs.Commands.Store.Interfaces;
using EssentialFrame.Cqrs.Commands.Store.Models;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Store;

internal sealed class CommandMapper : ICommandMapper
{
    private readonly ISerializer _serializer;

    public CommandMapper(ISerializer serializer)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public CommandDataModel Map(ICommand command)
    {
        return new CommandDataModel(command);
    }

    public IReadOnlyCollection<CommandDataModel> Map(IEnumerable<ICommand> commands)
    {
        return commands.Select(Map).ToList();
    }

    public ICommand Map(CommandDataModel commandDataModel)
    {
        object command = commandDataModel.Command;

        if (command is not string serializedCommand)
        {
            return command as ICommand;
        }

        ICommand deserialized =
            _serializer.Deserialize<ICommand>(serializedCommand, Type.GetType(commandDataModel.CommandClass));

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
}