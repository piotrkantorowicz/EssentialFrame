using EssentialFrame.Cqrs.Commands.Interfaces;
using EssentialFrame.Cqrs.Commands.Store;
using EssentialFrame.Cqrs.Interfaces;

namespace EssentialFrame.Cqrs.Executors;

public abstract class CommandExecutorBase : ICommandExecutor
{
    private readonly ICommandStore _commandStore;

    protected CommandExecutorBase(ICommandStore commandStore) =>
        _commandStore = commandStore ?? throw new ArgumentNullException(nameof(commandStore));

    public virtual ICommandResult Send<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        var handler = FindHandler<TCommand, ICommandHandler<TCommand>>(command);

        return handler.Handle(command);
    }

    public virtual ICommandResult SendAndStore<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        _commandStore.StartExecution(command);

        var handler = FindHandler<TCommand, ICommandHandler<TCommand>>(command);
        var commandResult = handler.Handle(command);

        _commandStore.CompleteExecution(command.CommandIdentifier, commandResult.IsSuccess);

        return commandResult;
    }

    public virtual void Schedule<TCommand>(TCommand command, DateTimeOffset at)
        where TCommand : class, ICommand
    {
        _commandStore.ScheduleExecution(command, at);
    }

    public virtual void CancelSending<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        _commandStore.CancelExecution(command.CommandIdentifier);
    }

    public virtual async Task<ICommandResult> SendAsync<TCommand>(TCommand command,
                                                                  CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        var handler = FindHandler<TCommand, IAsyncCommandHandler<TCommand>>(command);

        return await handler.HandleAsync(command, cancellationToken);
    }

    public virtual async Task<ICommandResult> SendAndStoreAsync<TCommand>(TCommand command,
                                                                          CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await _commandStore.StartExecutionAsync(command, cancellationToken);

        var handler = FindHandler<TCommand, IAsyncCommandHandler<TCommand>>(command);
        var commandResult = await handler.HandleAsync(command, cancellationToken);

        await _commandStore.CompleteExecutionAsync(command.CommandIdentifier,
                                                   commandResult.IsSuccess,
                                                   cancellationToken);

        return commandResult;
    }

    public virtual async Task ScheduleAsync<TCommand>(TCommand command,
                                                      DateTimeOffset at,
                                                      CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await _commandStore.ScheduleExecutionAsync(command,
                                                   at,
                                                   cancellationToken);
    }

    public virtual async Task CancelSendingAsync<TCommand>(TCommand command,
                                                           CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await _commandStore.CancelExecutionAsync(command.CommandIdentifier, cancellationToken);
    }

    protected abstract THandler FindHandler<TCommand, THandler>(TCommand command)
        where TCommand : class, ICommand
        where THandler : class, ICommandHandler;
}
