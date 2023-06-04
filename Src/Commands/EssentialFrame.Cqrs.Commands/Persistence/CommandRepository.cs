using System.Collections.ObjectModel;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Exceptions;
using EssentialFrame.Cqrs.Commands.Persistence.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence.Models;

namespace EssentialFrame.Cqrs.Commands.Persistence;

public sealed class CommandRepository : ICommandRepository
{
    private readonly ICommandMapper _commandMapper;
    private readonly ICommandStore _commandStore;

    public CommandRepository(ICommandStore commandStore, ICommandMapper commandMapper)
    {
        _commandStore = commandStore ?? throw new ArgumentNullException(nameof(commandStore));
        _commandMapper = commandMapper ?? throw new ArgumentNullException(nameof(commandMapper));
    }

    public void StartExecution(ICommand command)
    {
        CommandDataModel commandDataModel = _commandMapper.Map(command);
        commandDataModel.Start();

        _commandStore.Save(commandDataModel, true);
    }

    public async Task StartExecutionAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        CommandDataModel commandDataModel = _commandMapper.Map(command);
        commandDataModel.Start();

        await _commandStore.SaveAsync(commandDataModel, true, cancellationToken);
    }

    public void CancelExecution(Guid commandIdentifier)
    {
        CommandDataModel commandDataModel = _commandStore.Get(commandIdentifier);

        if (commandDataModel is null)
        {
            throw new CommandNotFoundException(commandIdentifier);
        }

        commandDataModel.Cancel();
        _commandStore.Save(commandDataModel, false);
    }

    public async Task CancelExecutionAsync(Guid commandIdentifier, CancellationToken cancellationToken = default)
    {
        CommandDataModel commandDataModel = await _commandStore.GetAsync(commandIdentifier, cancellationToken);

        if (commandDataModel is null)
        {
            throw new CommandNotFoundException(commandIdentifier);
        }

        commandDataModel.Cancel();

        await _commandStore.SaveAsync(commandDataModel, false, cancellationToken);
    }

    public void ScheduleExecution(ICommand command, DateTimeOffset at)
    {
        CommandDataModel commandDataModel = _commandMapper.Map(command);
        commandDataModel.Schedule(at);

        _commandStore.Save(commandDataModel, true);
    }

    public async Task ScheduleExecutionAsync(ICommand command, DateTimeOffset at,
        CancellationToken cancellationToken = default)
    {
        CommandDataModel commandDataModel = _commandMapper.Map(command);
        commandDataModel.Start();

        await _commandStore.SaveAsync(commandDataModel, true, cancellationToken);
    }

    public void CompleteExecution(Guid commandIdentifier, bool isSuccess)
    {
        CommandDataModel commandDataModel = _commandStore.Get(commandIdentifier);

        if (commandDataModel is null)
        {
            throw new CommandNotFoundException(commandIdentifier);
        }

        commandDataModel.Complete(isSuccess);
        _commandStore.Save(commandDataModel, false);
    }

    public async Task CompleteExecutionAsync(Guid commandIdentifier, bool isSuccess,
        CancellationToken cancellationToken = default)
    {
        CommandDataModel commandDataModel = await _commandStore.GetAsync(commandIdentifier, cancellationToken);

        if (commandDataModel is null)
        {
            throw new CommandNotFoundException(commandIdentifier);
        }

        commandDataModel.Complete(isSuccess);

        await _commandStore.SaveAsync(commandDataModel, false, cancellationToken);
    }

    public IReadOnlyCollection<ICommand> GetPossibleToSend(DateTimeOffset at)
    {
        return _commandStore.GetPossibleToSend(at).Select(c => _commandMapper.Map(c)).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyCollection<ICommand>> GetPossibleToSendAsync(DateTimeOffset at,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<CommandDataModel> possibleCommandsDataToSend =
            await _commandStore.GetPossibleToSendAsync(at, cancellationToken);

        ReadOnlyCollection<ICommand> possibleCommandsToSend =
            possibleCommandsDataToSend.Select(c => _commandMapper.Map(c)).ToList().AsReadOnly();

        return possibleCommandsToSend;
    }
}