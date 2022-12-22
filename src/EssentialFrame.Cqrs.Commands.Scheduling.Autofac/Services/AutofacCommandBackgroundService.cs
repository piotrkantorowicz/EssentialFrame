using Autofac;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Scheduling.Services;
using EssentialFrame.Cqrs.Commands.Store;

namespace EssentialFrame.Cqrs.Commands.Scheduling.Autofac.Services;

internal sealed class AutofacCommandBackgroundService : CommandBackgroundServiceBase
{
    private readonly ILifetimeScope _lifetimeScope;

    public AutofacCommandBackgroundService(ILifetimeScope lifetimeScope,
                                           int timeInterval)
        : base(timeInterval) =>
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();

        var commandStore = scope.Resolve<ICommandStore>();
        var commandDispatcher = scope.Resolve<ICommandExecutor>();

        await ProcessUnsentCommands(commandStore,
                                    commandDispatcher,
                                    stoppingToken);
    }
}
