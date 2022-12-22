using EssentialFrame.Cqrs.Commands.Core.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Scheduling.Core;

public interface ICommandStoreExecutor
{
    ICommandResult SendAndStore<TCommand>(TCommand command)
        where TCommand : class, ICommand;

    Task<ICommandResult> SendAndStoreAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;
}
