namespace EssentialFrame.Cqrs.Commands.Core.Interfaces;

public interface IAsyncCommandHandler<in TCommand> : ICommandHandler where TCommand : class, ICommand
{
    Task<ICommandResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand> : ICommandHandler where TCommand : class, ICommand
{
    ICommandResult Handle(TCommand command);
}

public interface ICommandHandler
{
}