using System.Collections.ObjectModel;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Exceptions;
using EssentialFrame.Cqrs.Commands.Store.Interfaces;
using EssentialFrame.Cqrs.Commands.Store.Models;
using EssentialFrame.Serialization.Interfaces;

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
        CommandDao commandDao = new(command);
        commandDao.Start();

        _commandStore.Save(commandDao, true);
    }

    public async Task StartExecutionAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        CommandDao commandDao = new(command);
        commandDao.Start();

        await _commandStore.SaveAsync(commandDao, true, cancellationToken);
    }

    public void CancelExecution(Guid commandIdentifier)
    {
        CommandDao commandDao = _commandStore.Get(commandIdentifier);

        if (commandDao is null)
        {
            throw new CommandNotFoundException(commandIdentifier);
        }

        commandDao.Cancel();
        _commandStore.Save(commandDao, false);
    }

    public async Task CancelExecutionAsync(Guid commandIdentifier, CancellationToken cancellationToken = default)
    {
        CommandDao commandDao = await _commandStore.GetAsync(commandIdentifier, cancellationToken);

        if (commandDao is null)
        {
            throw new CommandNotFoundException(commandIdentifier);
        }

        commandDao.Cancel();

        await _commandStore.SaveAsync(commandDao, false, cancellationToken);
    }

    public void ScheduleExecution(ICommand command, DateTimeOffset at)
    {
        CommandDao commandDao = new(command);
        commandDao.Schedule(at);

        _commandStore.Save(commandDao, true);
    }

    public async Task ScheduleExecutionAsync(ICommand command, DateTimeOffset at,
        CancellationToken cancellationToken = default)
    {
        CommandDao commandDao = new(command);
        commandDao.Start();

        await _commandStore.SaveAsync(commandDao, true, cancellationToken);
    }

    public void CompleteExecution(Guid commandIdentifier, bool isSuccess)
    {
        CommandDao commandDao = _commandStore.Get(commandIdentifier);

        if (commandDao is null)
        {
            throw new CommandNotFoundException(commandIdentifier);
        }

        commandDao.Complete(isSuccess);
        _commandStore.Save(commandDao, false);
    }

    public async Task CompleteExecutionAsync(Guid commandIdentifier, bool isSuccess,
        CancellationToken cancellationToken = default)
    {
        CommandDao commandDao = await _commandStore.GetAsync(commandIdentifier, cancellationToken);

        if (commandDao is null)
        {
            throw new CommandNotFoundException(commandIdentifier);
        }

        commandDao.Complete(isSuccess);

        await _commandStore.SaveAsync(commandDao, false, cancellationToken);
    }

    public IReadOnlyCollection<ICommand> GetPossibleToSend(DateTimeOffset at)
    {
        return _commandStore.GetPossibleToSend(at).Select(ConvertToCommand).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyCollection<ICommand>> GetPossibleToSendAsync(DateTimeOffset at,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<CommandDao> possibleCommandsDataToSend =
            await _commandStore.GetPossibleToSendAsync(at, cancellationToken);

        ReadOnlyCollection<ICommand> possibleCommandsToSend =
            possibleCommandsDataToSend.Select(ConvertToCommand).ToList().AsReadOnly();

        return possibleCommandsToSend;
    }

    private ICommand ConvertToCommand(CommandDao commandDao)
    {
        object command = commandDao.Command;

        if (command is string serializedEvent)
        {
            ICommand deserialized =
                _serializer.Deserialize<ICommand>(serializedEvent, Type.GetType(commandDao.CommandClass));

            if (deserialized is null)
            {
                throw new UnknownCommandTypeException(commandDao.CommandType);
            }
        }

        ICommand castedCommand = command as ICommand;

        return castedCommand;
    }
}