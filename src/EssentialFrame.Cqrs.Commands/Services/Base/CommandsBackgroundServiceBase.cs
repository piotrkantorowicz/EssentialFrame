using EssentialFrame.Background;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Executors.Interfaces;
using EssentialFrame.Time;

namespace EssentialFrame.Cqrs.Commands.Services.Base;

public abstract class CommandsBackgroundServiceBase : BackgroundService
{
    private readonly int? _timeInterval;

    protected CommandsBackgroundServiceBase(int? timeInterval)
    {
        _timeInterval = timeInterval;
    }

    protected async Task ProcessUnsentCommands(ICommandRepository commandsRepository, ICommandExecutor commandExecutor,
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
                await commandsRepository.GetPossibleToSendAsync(SystemClock.Now, stoppingToken);

            if (storedCommandsPossibleToSend.Any())
            {
                foreach (ICommand storedCommand in storedCommandsPossibleToSend)
                {
                    await commandExecutor.SendAsync(storedCommand, stoppingToken);
                }
            }

            await Task.Delay(_timeInterval ?? 5000, stoppingToken);
        }
    }
}