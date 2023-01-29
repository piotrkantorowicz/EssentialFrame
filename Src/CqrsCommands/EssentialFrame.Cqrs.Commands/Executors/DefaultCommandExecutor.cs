using Autofac;
using Autofac.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Executors.Interfaces;
using EssentialFrame.Extensions;

namespace EssentialFrame.Cqrs.Commands.Executors;

internal sealed class DefaultCommandExecutor : ICommandExecutor, ICommandScheduler
{
    private readonly ILifetimeScope _lifetimeScope;

    public DefaultCommandExecutor(ILifetimeScope lifetimeScope)
    {
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
    }

    public ICommandResult Send<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();
        ICommandHandler<TCommand> handler = FindHandler<TCommand, ICommandHandler<TCommand>>(command, scope);

        return handler.Handle(command);
    }

    public async Task<ICommandResult> SendAsync<TCommand>(TCommand command,
        CancellationToken cancellationToken = default) where TCommand : class, ICommand
    {
        await using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();
        IAsyncCommandHandler<TCommand> handler = FindHandler<TCommand, IAsyncCommandHandler<TCommand>>(command, scope);

        return await handler.HandleAsync(command, cancellationToken);
    }

    public ICommandResult SendAndStore<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();
        ICommandRepository commandStore = GetCommandsRepository(scope);

        commandStore.StartExecution(command);

        ICommandHandler<TCommand> handler = FindHandler<TCommand, ICommandHandler<TCommand>>(command, scope);
        ICommandResult commandResult = handler.Handle(command);

        commandStore.CompleteExecution(command.CommandIdentifier, commandResult.IsSuccess);

        return commandResult;
    }

    public async Task<ICommandResult> SendAndStoreAsync<TCommand>(TCommand command,
        CancellationToken cancellationToken = default) where TCommand : class, ICommand
    {
        await using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();
        ICommandRepository commandStore = GetCommandsRepository(scope);

        await commandStore.StartExecutionAsync(command, cancellationToken);

        IAsyncCommandHandler<TCommand> handler = FindHandler<TCommand, IAsyncCommandHandler<TCommand>>(command, scope);
        ICommandResult commandResult = await handler.HandleAsync(command, cancellationToken);

        await commandStore.CompleteExecutionAsync(command.CommandIdentifier, commandResult.IsSuccess,
            cancellationToken);

        return commandResult;
    }

    public void Schedule<TCommand>(TCommand command, DateTimeOffset at) where TCommand : class, ICommand
    {
        using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();
        ICommandRepository commandStore = GetCommandsRepository(scope);

        commandStore.ScheduleExecution(command, at);
    }

    public void CancelFromSchedule<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();
        ICommandRepository commandStore = GetCommandsRepository(scope);

        commandStore.CancelExecution(command.CommandIdentifier);
    }

    public async Task ScheduleAsync<TCommand>(TCommand command, DateTimeOffset at,
        CancellationToken cancellationToken = default) where TCommand : class, ICommand
    {
        await using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();
        ICommandRepository commandStore = GetCommandsRepository(scope);

        await commandStore.ScheduleExecutionAsync(command, at, cancellationToken);
    }

    public async Task CancelFromScheduleAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();
        ICommandRepository commandStore = GetCommandsRepository(scope);

        await commandStore.CancelExecutionAsync(command.CommandIdentifier, cancellationToken);
    }

    private static ICommandRepository GetCommandsRepository(IComponentContext lifetimeScope)
    {
        bool isCommandsRepositoryResolved = lifetimeScope.TryResolve(out ICommandRepository commandsRepository);

        if (!isCommandsRepositoryResolved)
        {
            throw new DependencyResolutionException($"Unable to resolve {commandsRepository.GetTypeFullName()}. " +
                                                    "Most likely it is not properly registered in container.");
        }

        return commandsRepository;
    }

    private static THandler FindHandler<TCommand, THandler>(TCommand command, IComponentContext lifetimeScope)
        where TCommand : class, ICommand where THandler : class, ICommandHandler
    {
        bool isTenantHandlerFound = lifetimeScope.TryResolveKeyed(command.TenantIdentity, out THandler commandHandler);

        if (isTenantHandlerFound)
        {
            return commandHandler;
        }

        bool isGeneralHandlerFound = lifetimeScope.TryResolve(out commandHandler);

        if (isGeneralHandlerFound)
        {
            return commandHandler;
        }

        throw new DependencyResolutionException($"Unable to resolve {command.GetTypeFullName()}. " +
                                                "Most likely it is not properly registered in container.");
    }
}