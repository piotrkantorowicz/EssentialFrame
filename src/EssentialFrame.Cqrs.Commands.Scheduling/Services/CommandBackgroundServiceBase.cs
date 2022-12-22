using EssentialFrame.BackgroundTasks;
using EssentialFrame.Core.Time;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Store;

namespace EssentialFrame.Cqrs.Commands.Scheduling.Services;

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
            var storedCommandsPossibleToSend =
                await commandStore.GetPossibleToSendAsync(SystemClock.Now, stoppingToken);

            if (storedCommandsPossibleToSend.Any())
            {
                foreach (var storedCommand in storedCommandsPossibleToSend)
                {
                    var unSendCommand = commandStore.ConvertToCommand(storedCommand);

                    await commandExecutor.SendAsync(unSendCommand, stoppingToken);
                }
            }

            await Task.Delay(_timeInterval, stoppingToken);
        }
    }
}

