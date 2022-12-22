using Autofac;
using Autofac.Core;
using EssentialFrame.Core.Extensions;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Scheduling.Core;
using EssentialFrame.Cqrs.Commands.Store;

namespace EssentialFrame.Cqrs.Commands.Scheduling.Autofac.Schedulers;

internal sealed class AutofacCommandScheduler : ICommandScheduler
{
    private readonly ILifetimeScope _lifetimeScope;

    public AutofacCommandScheduler(ILifetimeScope lifetimeScope) =>
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));

    public void Schedule<TCommand>(TCommand command, DateTimeOffset at)
        where TCommand : class, ICommand
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();
        var commandStore = GetCommandStore(scope);

        commandStore.ScheduleExecution(command, at);
    }

    public void CancelFromSchedule<TCommand>(TCommand command)
        where TCommand : class, ICommand
    {
        using var scope = _lifetimeScope.BeginLifetimeScope();
        var commandStore = GetCommandStore(scope);

        commandStore.CancelExecution(command.CommandIdentifier);
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

    public async Task CancelFromScheduleAsync<TCommand>(TCommand command,
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
}
