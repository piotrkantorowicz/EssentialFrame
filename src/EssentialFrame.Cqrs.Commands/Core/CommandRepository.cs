using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Exceptions;
using EssentialFrame.Cqrs.Commands.Store;
using EssentialFrame.Cqrs.Commands.Store.Models;
using EssentialFrame.Serialization;

namespace EssentialFrame.Cqrs.Commands.Core;

public sealed class CommandRepository : ICommandRepository
{
    private readonly ICommandStore _commandStore;
    private readonly ISerializer _serializer;

    public CommandRepository(ICommandStore commandStore, ISerializer serializer)
    {
        _commandStore = commandStore ?? throw new ArgumentNullException(nameof(commandStore));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public void StartExecution(ICommand command)
    {
        var commandData = new CommandData(command);
        commandData.Start();

        _commandStore.Save(commandData, true);
    }

    public async Task StartExecutionAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        var commandData = new CommandData(command);
        commandData.Start();

        await _commandStore.SaveAsync(commandData,
                                      true,
                                      cancellationToken);
    }

    public void CancelExecution(Guid commandIdentifier)
    {
        var commandData = _commandStore.Get(commandIdentifier);

        if (commandData is null)
        {
            throw new CommandNotFoundException(commandIdentifier);
        }

        commandData.Cancel();
        _commandStore.Save(commandData, false);
    }

    public async Task CancelExecutionAsync(Guid commandIdentifier, CancellationToken cancellationToken = default)
    {
        var commandData = await _commandStore.GetAsync(commandIdentifier, cancellationToken);

        if (commandData is null)
        {
            throw new CommandNotFoundException(commandIdentifier);
        }

        commandData.Cancel();

        await _commandStore.SaveAsync(commandData,
                                      false,
                                      cancellationToken);
    }

    public void ScheduleExecution(ICommand command, DateTimeOffset at)
    {
        var commandData = new CommandData(command);
        commandData.Schedule(at);

        _commandStore.Save(commandData, true);
    }

    public async Task ScheduleExecutionAsync(ICommand command,
                                             DateTimeOffset at,
                                             CancellationToken cancellationToken = default)
    {
        var commandData = new CommandData(command);
        commandData.Start();

        await _commandStore.SaveAsync(commandData,
                                      true,
                                      cancellationToken);
    }

    public void CompleteExecution(Guid commandIdentifier, bool isSuccess)
    {
        var commandData = _commandStore.Get(commandIdentifier);

        if (commandData is null)
        {
            throw new CommandNotFoundException(commandIdentifier);
        }

        commandData.Complete(isSuccess);
        _commandStore.Save(commandData, false);
    }

    public async Task CompleteExecutionAsync(Guid commandIdentifier,
                                             bool isSuccess,
                                             CancellationToken cancellationToken = default)
    {
        var commandData = await _commandStore.GetAsync(commandIdentifier, cancellationToken);

        if (commandData is null)
        {
            throw new CommandNotFoundException(commandIdentifier);
        }

        commandData.Complete(isSuccess);

        await _commandStore.SaveAsync(commandData,
                                      false,
                                      cancellationToken);
    }

    public IReadOnlyCollection<ICommand> GetPossibleToSend(DateTimeOffset at) =>
        _commandStore
            .GetPossibleToSend(at)
            .Select(ConvertToEvent)
            .ToList()
            .AsReadOnly();

    public async Task<IReadOnlyCollection<ICommand>>
        GetPossibleToSendAsync(DateTimeOffset at, CancellationToken cancellationToken = default)
    {
        var possibleCommandsDataToSend = await _commandStore.GetPossibleToSendAsync(at, cancellationToken);

        var possibleCommandsToSend = possibleCommandsDataToSend
                                     .Select(ConvertToEvent)
                                     .ToList()
                                     .AsReadOnly();

        return possibleCommandsToSend;
    }

    private ICommand ConvertToEvent(CommandData commandData)
    {
        var command = commandData.Command;

        if (command is string serializedEvent)
        {
            var deserialized =
                _serializer.Deserialize<ICommand>(serializedEvent, Type.GetType(commandData.CommandClass));

            if (deserialized is null)
            {
                throw new UnknownCommandTypeException(commandData.CommandType);
            }
        }

        var castedCommand = command as ICommand;

        return castedCommand;
    }
}
