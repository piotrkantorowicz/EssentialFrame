using Autofac;
using EssentialFrame.Background;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Persistence.Interfaces;
using EssentialFrame.Cqrs.Commands.Services.Execution.Interfaces;
using EssentialFrame.Time;

namespace EssentialFrame.Cqrs.Commands.Services.Scheduling;

internal sealed class DefaultCommandsScheduler : BackgroundService
{
    private readonly ILifetimeScope _lifetimeScope;
    private readonly int _timeIntervalMilliSeconds;

    public DefaultCommandsScheduler(ILifetimeScope lifetimeScope, int timeIntervalMilliSeconds)
    {
        if (timeIntervalMilliSeconds <= 100)
        {
            throw new ArgumentOutOfRangeException(nameof(timeIntervalMilliSeconds), timeIntervalMilliSeconds,
                "Interval value must be greater that 100 ms.");
        }

        _timeIntervalMilliSeconds = timeIntervalMilliSeconds;
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using ILifetimeScope scope = _lifetimeScope.BeginLifetimeScope();

        ICommandRepository commandsRepository = scope.Resolve<ICommandRepository>();
        ICommandExecutor commandDispatcher = scope.Resolve<ICommandExecutor>();

        await ProcessUnsentCommands(commandsRepository, commandDispatcher, stoppingToken);
    }

    private async Task ProcessUnsentCommands(ICommandRepository commandsRepository, ICommandExecutor commandExecutor,
        CancellationToken stoppingToken)
    {
        if (commandsRepository == null)
        {
            throw new ArgumentNullException(nameof(commandsRepository));
        }

        if (commandExecutor == null)
        {
            throw new ArgumentNullException(nameof(commandExecutor));
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            IReadOnlyCollection<ICommand> storedCommandsPossibleToSend =
                await commandsRepository.GetPossibleToSendAsync(SystemClock.UtcNow, stoppingToken);

            if (storedCommandsPossibleToSend.Any())
            {
                foreach (ICommand storedCommand in storedCommandsPossibleToSend)
                {
                    await commandExecutor.SendAsync(storedCommand, stoppingToken);
                }
            }

            await Task.Delay(_timeIntervalMilliSeconds, stoppingToken);
        }
    }
}