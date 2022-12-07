using EssentialFrame.Cqrs.Commands.Core;

namespace EssentialFrame.Cqrs.Interfaces;

public interface ICommandHandler<in TCommand>
    where TCommand : class, ICommand
{
    ICommandResult Handle(TCommand command);

    Task<ICommandResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
