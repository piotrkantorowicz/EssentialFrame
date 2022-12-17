using Autofac;
using EssentialFrame.Cqrs.Commands.BackgroundProcessing.Services;
using EssentialFrame.Cqrs.Commands.Interfaces;
using EssentialFrame.Cqrs.Commands.Store;

namespace EssentialFrame.Cqrs.Commands.BackgroundProcessing.Autofac.Services;

public class AutofacCommandBackgroundService : CommandBackgroundServiceBase
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

