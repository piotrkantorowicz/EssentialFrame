using EssentialFrame.BackgroundTasks;
using EssentialFrame.Core;
using EssentialFrame.Cqrs.Commands.Interfaces;
using EssentialFrame.Cqrs.Commands.Store;

namespace EssentialFrame.Cqrs.Commands.BackgroundProcessing.Services;

public abstract class CommandBackgroundServiceBase : BackgroundService
{
    private readonly int _timeInterval;

    protected CommandBackgroundServiceBase(int timeInterval) => _timeInterval = timeInterval;

    protected async Task ProcessUnsentCommands(ICommandStore commandStore,
                                               ICommandExecutor commandExecutor,
                                               CancellationToken stoppingToken)
    {
        if (commandStore == null)
        {
            throw new ArgumentNullException(nameof(commandStore));
        }

        if (commandExecutor == null)
        {
            throw new ArgumentNullException(nameof(commandExecutor));
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            var commandsPossibleToSend = await commandStore.GetPossibleToSendAsync(SystemClock.Now, stoppingToken);

            if (commandsPossibleToSend.Any())
            {
                foreach (var unSendCommand in commandsPossibleToSend)
                {
                    await commandExecutor.SendAsync(unSendCommand, stoppingToken);
                }
            }

            await Task.Delay(_timeInterval, stoppingToken);
        }
    }
}

