using EssentialFrame.Cqrs.Commands.Core.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Executors.Interfaces;

public interface ICommandExecutor
{
    ICommandResult Send<TCommand>(TCommand command) where TCommand : class, ICommand;

    Task<ICommandResult> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    ICommandResult SendAndStore<TCommand>(TCommand command) where TCommand : class, ICommand;

    Task<ICommandResult> SendAndStoreAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;
}