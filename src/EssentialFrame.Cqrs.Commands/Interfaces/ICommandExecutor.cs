namespace EssentialFrame.Cqrs.Commands.Interfaces;

public interface ICommandExecutor
{
    ICommandResult Send<TCommand>(TCommand command)
        where TCommand : class, ICommand;

    ICommandResult SendAndStore<TCommand>(TCommand command)
        where TCommand : class, ICommand;

    void Schedule<TCommand>(TCommand command, DateTimeOffset at)
        where TCommand : class, ICommand;

    void CancelSending<TCommand>(TCommand command)
        where TCommand : class, ICommand;

    Task<ICommandResult> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    Task<ICommandResult> SendAndStoreAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    Task ScheduleAsync<TCommand>(TCommand command,
                                 DateTimeOffset at,
                                 CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    Task CancelSendingAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;
}

