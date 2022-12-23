using Autofac;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Services;

namespace EssentialFrame.Cqrs.Commands.Autofac.Services;

internal sealed class AutofacCommandBackgroundService : CommandBackgroundServiceBase
{
    private readonly ILifetimeScope _lifetimeScope;

    public AutofacCommandBackgroundService(ILifetimeScope lifetimeScope, int timeInterval)
        : base(timeInterval) =>
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _lifetimeScope.BeginLifetimeScope();

        var commandsRepository = scope.Resolve<ICommandRepository>();
        var commandDispatcher = scope.Resolve<ICommandExecutor>();

        await ProcessUnsentCommands(commandsRepository,
                                    commandDispatcher,
                                    stoppingToken);
    }
}
