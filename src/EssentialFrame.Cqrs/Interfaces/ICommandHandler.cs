using EssentialFrame.Cqrs.Commands.Interfaces;

namespace EssentialFrame.Cqrs.Interfaces;

public interface ICommandHandler<in TCommand> : IHandler
    where TCommand : class, ICommand
{
    ICommandResult Handle(TCommand command);
}
