using Autofac;
using EssentialFrame.Cqrs.Commands.Persistence.Interfaces;
using EssentialFrame.Cqrs.Commands.Services.Execution.Interfaces;
using EssentialFrame.Cqrs.Commands.Services.Scheduling.Base;

namespace EssentialFrame.Cqrs.Commands.Services.Scheduling;

internal sealed class DefaultCommandsScheduler : DefaultCommandSchedulerBase
{
    private readonly ILifetimeScope _lifetimeScope;

    public DefaultCommandsScheduler(ILifetimeScope lifetimeScope, int timeInterval) : base(timeInterval)
    {
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        ICommandRepository commandsRepository = scope.Resolve<ICommandRepository>();
        ICommandExecutor commandDispatcher = scope.Resolve<ICommandExecutor>();

        await ProcessUnsentCommands(commandsRepository, commandDispatcher, stoppingToken);
    }
}