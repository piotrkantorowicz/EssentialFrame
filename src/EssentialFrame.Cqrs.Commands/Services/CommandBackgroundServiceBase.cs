using EssentialFrame.BackgroundTasks;
using EssentialFrame.Core.Time;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Services;

public abstract class CommandBackgroundServiceBase : BackgroundService
{
    private readonly int _timeInterval;

    protected CommandBackgroundServiceBase(int timeInterval) => _timeInterval = timeInterval;

    protected async Task ProcessUnsentCommands(ICommandRepository commandsRepository,
                                               ICommandExecutor commandExecutor,
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
            var storedCommandsPossibleToSend =
                await commandsRepository.GetPossibleToSendAsync(SystemClock.Now, stoppingToken);

            if (storedCommandsPossibleToSend.Any())
            {
                foreach (var storedCommand in storedCommandsPossibleToSend)
                {
                    await commandExecutor.SendAsync(storedCommand, stoppingToken);
                }
            }

            await Task.Delay(_timeInterval, stoppingToken);
        }
    }
}
