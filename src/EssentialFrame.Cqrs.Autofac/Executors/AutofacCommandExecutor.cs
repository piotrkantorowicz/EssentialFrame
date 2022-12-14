using Autofac;
using Autofac.Core;
using EssentialFrame.Core.Extensions;
using EssentialFrame.Cqrs.Commands.Interfaces;
using EssentialFrame.Cqrs.Commands.Store;
using EssentialFrame.Cqrs.Interfaces;

namespace EssentialFrame.Cqrs.Autofac.Executors;

internal sealed class AutofacCommandExecutor : ICommandExecutor
{
    private readonly ILifetimeScope _lifetimeScope;

    public AutofacCommandExecutor(ILifetimeScope lifetimeScope) =>
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));

    public ICommandResult Send<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();
        var handler = FindHandler<TCommand, ICommandHandler<TCommand>>(command, scope);

        return handler.Handle(command);
    }

    public ICommandResult SendAndStore<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();
        var commandStore = GetCommandStore(scope);

        commandStore.StartExecution(command);

        var handler = FindHandler<TCommand, ICommandHandler<TCommand>>(command, scope);
        var commandResult = handler.Handle(command);

        commandStore.CompleteExecution(command.CommandIdentifier, commandResult.IsSuccess);

        return commandResult;
    }

    public void Schedule<TCommand>(TCommand command, DateTimeOffset at)
        where TCommand : class, ICommand
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();
        var commandStore = GetCommandStore(scope);

        commandStore.ScheduleExecution(command, at);
    }

    public void CancelSending<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();
        var commandStore = GetCommandStore(scope);

        commandStore.CancelExecution(command.CommandIdentifier);
    }

    public async Task<ICommandResult> SendAsync<TCommand>(TCommand command,
                                                          CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();
        var handler = FindHandler<TCommand, IAsyncCommandHandler<TCommand>>(command, scope);

        return await handler.HandleAsync(command, cancellationToken);
    }

    public async Task<ICommandResult> SendAndStoreAsync<TCommand>(TCommand command,
                                                                  CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();
        var commandStore = GetCommandStore(scope);

        await commandStore.StartExecutionAsync(command, cancellationToken);

        var handler = FindHandler<TCommand, IAsyncCommandHandler<TCommand>>(command, scope);
        var commandResult = await handler.HandleAsync(command, cancellationToken);

        await commandStore.CompleteExecutionAsync(command.CommandIdentifier,
                                                  commandResult.IsSuccess,
                                                  cancellationToken);

        return commandResult;
    }

    public async Task ScheduleAsync<TCommand>(TCommand command,
                                              DateTimeOffset at,
                                              CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();
        var commandStore = GetCommandStore(scope);

        await commandStore.ScheduleExecutionAsync(command,
                                                  at,
                                                  cancellationToken);
    }

    public async Task CancelSendingAsync<TCommand>(TCommand command,
                                                   CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();
        var commandStore = GetCommandStore(scope);

        await commandStore.CancelExecutionAsync(command.CommandIdentifier, cancellationToken);
    }

    private static ICommandStore GetCommandStore(IComponentContext lifetimeScope)
    {
        var isCommandStoreResolved = lifetimeScope.TryResolve(out ICommandStore commandStore);

        if (!isCommandStoreResolved)
        {
            throw new
                DependencyResolutionException($"Unable to resolve {commandStore.GetTypeFullName()}. " +
                                              "Most likely it is not properly registered in container.");
        }

        return commandStore;
    }

    private static THandler FindHandler<TCommand, THandler>(TCommand command, IComponentContext lifetimeScope)
        where TCommand : class, ICommand
        where THandler : class, ICommandHandler
    {
        var isTenantHandlerFound =
            lifetimeScope.TryResolveKeyed(command.TenantIdentity, out THandler commandHandler);

        if (isTenantHandlerFound)
        {
            return commandHandler;
        }

        var isGeneralHandlerFound = lifetimeScope.TryResolve(out commandHandler);

        if (isGeneralHandlerFound)
        {
            return commandHandler;
        }

        throw new
            DependencyResolutionException($"Unable to resolve {command.GetTypeFullName()}. " +
                                          "Most likely it is not properly registered in container.");
    }
}
