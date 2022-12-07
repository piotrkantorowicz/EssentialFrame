using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Interfaces;

namespace EssentialFrame.Cqrs;

public abstract class CommandDispatcherBase : ICommandDispatcher
{
    private readonly ICommandStore _commandStore;

    protected CommandDispatcherBase(ICommandStore commandStore) =>
        _commandStore = commandStore ?? throw new ArgumentNullException(nameof(commandStore));

    public abstract ICommandResult Send<TCommand>(TCommand command)
        where TCommand : class, ICommand;

    public abstract Task<ICommandResult> SendAsync<TCommand>(TCommand command,
                                                             CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    public abstract ICommandResult Schedule<TCommand>(TCommand command, DateTimeOffset at)
        where TCommand : class, ICommand;

    public abstract Task<ICommandResult> ScheduleAsync<TCommand>(TCommand command,
                                                                 DateTimeOffset at,
                                                                 CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    protected abstract ICommandHandler<TCommand> FindHandler<TCommand>()
        where TCommand : class, ICommand;

    protected void StartExecution<TCommand>(TCommand command)
    {
    }

    protected Task StartExecutionAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
    {
    }

    protected void CancelExecution<TCommand>(TCommand command)
    {
    }

    protected Task CancelExecutionAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
    {
    }

    protected void ScheduleExecution<TCommand>(TCommand command)
    {
    }

    protected Task ScheduleExecutionAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
    {
    }

    protected void CompleteExecution<TCommand>(TCommand command)
    {
    }

    protected Task CompleteExecutionAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
    {
    }
}
