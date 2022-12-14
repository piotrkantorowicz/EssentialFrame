using EssentialFrame.Cqrs.Commands.Interfaces;

namespace EssentialFrame.Cqrs.Interfaces;

public interface IAsyncCommandHandler<in TCommand> : IHandler
    where TCommand : class, ICommand
{
    Task<ICommandResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
