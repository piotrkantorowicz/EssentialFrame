using Autofac;
using EssentialFrame.Cqrs.Autofac.Exceptions;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Interfaces;

namespace EssentialFrame.Cqrs.Autofac.Dispatchers;

internal sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly ICommandStore _commandStore;
    private readonly ILifetimeScope _lifetimeScope;

    public CommandDispatcher(ILifetimeScope lifetimeScope, ICommandStore commandStore)
    {
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        _commandStore = commandStore ?? throw new ArgumentNullException(nameof(commandStore));
    }

    public async Task<ICommandResult> SendAsync<TCommand>(TCommand command,
                                                          CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();

        var commandHandler = FindHandler(command, scope);

        return await commandHandler.HandleAsync(command, cancellationToken);
    }

    private static ICommandHandler<TCommand> FindHandler<TCommand>(TCommand command,
                                                                   IComponentContext lifetimeScope)
        where TCommand : class, ICommand
    {
        var isTenantHandlerFound =
            lifetimeScope.TryResolveKeyed(command.IdentityTenant, out ICommandHandler<TCommand> commandHandler);

        if (isTenantHandlerFound)
        {
            return commandHandler;
        }

        var isGeneralHandlerFound = lifetimeScope.TryResolve(out commandHandler);

        if (isGeneralHandlerFound)
        {
            return commandHandler;
        }

        throw new HandlerNotFoundException(command.GetType().Name);
    }
}
