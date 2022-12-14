using Autofac;
using EssentialFrame.Core.Extensions;
using EssentialFrame.Cqrs.Autofac.Exceptions;
using EssentialFrame.Cqrs.Commands.Store;
using EssentialFrame.Cqrs.Executors;

namespace EssentialFrame.Cqrs.Autofac.Executors;

internal sealed class AutofacCommandExecutor : CommandExecutorBase
{
    private readonly ILifetimeScope _lifetimeScope;

    public AutofacCommandExecutor(ICommandStore commandStore, ILifetimeScope lifetimeScope)
        : base(commandStore) =>
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));

    protected override THandler FindHandler<TCommand, THandler>(TCommand command)
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();

        var isTenantHandlerFound =
            scope.TryResolveKeyed(command.TenantIdentity, out THandler commandHandler);

        if (isTenantHandlerFound)
        {
            return commandHandler;
        }

        var isGeneralHandlerFound = scope.TryResolve(out commandHandler);

        if (isGeneralHandlerFound)
        {
            return commandHandler;
        }

        throw new HandlerNotFoundException(command.GetTypeFullName());
    }
}
