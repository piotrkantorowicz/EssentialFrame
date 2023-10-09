using Autofac;
using Autofac.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence.Interfaces;
using EssentialFrame.Cqrs.Commands.Services.Execution.Interfaces;
using EssentialFrame.Extensions;
using EssentialFrame.Serialization.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Services.Execution;

internal sealed class DefaultCommandExecutor : ICommandExecutor, ICommandScheduler
{
    private readonly ILifetimeScope _lifetimeScope;
    private readonly ICommandRepository _commandsRepository;

    public DefaultCommandExecutor(ILifetimeScope lifetimeScope, ICommandRepository commandRepository)
    {
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        _commandsRepository = commandRepository ?? throw new ArgumentNullException(nameof(commandRepository));
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

        _commandsRepository.StartExecution(command);

        ICommandHandler<TCommand> handler = FindHandler<TCommand, ICommandHandler<TCommand>>(command, scope);
        ICommandResult commandResult = handler.Handle(command);

        _commandsRepository.CompleteExecution(command.CommandIdentifier, commandResult.IsSuccess);

        return commandResult;
    }

    public async Task<ICommandResult> SendAndStoreAsync<TCommand>(TCommand command,
        CancellationToken cancellationToken = default) where TCommand : class, ICommand
    {
        await using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        await _commandsRepository.StartExecutionAsync(command, cancellationToken);

        IAsyncCommandHandler<TCommand> handler = FindHandler<TCommand, IAsyncCommandHandler<TCommand>>(command, scope);
        ICommandResult commandResult = await handler.HandleAsync(command, cancellationToken);

        await _commandsRepository.CompleteExecutionAsync(command.CommandIdentifier, commandResult.IsSuccess,
            cancellationToken);

        return commandResult;
    }

    public void Schedule<TCommand>(TCommand command, DateTimeOffset at) where TCommand : class, ICommand
    {
        using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        _commandsRepository.ScheduleExecution(command, at);
    }

    public void CancelFromSchedule<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        _commandsRepository.CancelExecution(command.CommandIdentifier);
    }

    public async Task ScheduleAsync<TCommand>(TCommand command, DateTimeOffset at,
        CancellationToken cancellationToken = default) where TCommand : class, ICommand
    {
        await using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        await _commandsRepository.ScheduleExecutionAsync(command, at, cancellationToken);
    }

    public async Task CancelFromScheduleAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        await using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        await _commandsRepository.CancelExecutionAsync(command.CommandIdentifier, cancellationToken);
    }

    public ICommandResult SendAndStore<TCommand>(TCommand command, ISerializer serializer)
        where TCommand : class, ICommand
    {
        using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        _commandsRepository.StartExecution(command, serializer);

        ICommandHandler<TCommand> handler = FindHandler<TCommand, ICommandHandler<TCommand>>(command, scope);
        ICommandResult commandResult = handler.Handle(command);

        _commandsRepository.CompleteExecution(command.CommandIdentifier, commandResult.IsSuccess);

        return commandResult;
    }

    public async Task<ICommandResult> SendAndStoreAsync<TCommand>(TCommand command, ISerializer serializer,
        CancellationToken cancellationToken = default) where TCommand : class, ICommand
    {
        await using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        await _commandsRepository.StartExecutionAsync(command, serializer, cancellationToken);

        IAsyncCommandHandler<TCommand> handler = FindHandler<TCommand, IAsyncCommandHandler<TCommand>>(command, scope);
        ICommandResult commandResult = await handler.HandleAsync(command, cancellationToken);

        await _commandsRepository.CompleteExecutionAsync(command.CommandIdentifier, commandResult.IsSuccess,
            cancellationToken);

        return commandResult;
    }

    private static THandler FindHandler<TCommand, THandler>(TCommand command, IComponentContext lifetimeScope)
        where TCommand : class, ICommand where THandler : class, ICommandHandler
    {
        bool isTenantHandlerFound =
            lifetimeScope.TryResolveKeyed(command.IdentityContext.Tenant.Identifier, out THandler commandHandler);

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
                                                "Most likely it is not properly registered in container");
    }
}