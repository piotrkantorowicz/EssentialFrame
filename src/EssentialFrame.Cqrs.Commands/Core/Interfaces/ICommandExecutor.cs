namespace EssentialFrame.Cqrs.Commands.Core.Interfaces;

public interface ICommandExecutor
{
    ICommandResult Send<TCommand>(TCommand command)
        where TCommand : class, ICommand;

    Task<ICommandResult> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;
}


