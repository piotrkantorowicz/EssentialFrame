using EssentialFrame.BackgroundTasks.Interfaces;
using Microsoft.Extensions.Hosting;

namespace EssentialFrame.BackgroundTasks;

public abstract class BackgroundService : IHostedService, IBackgroundService, IDisposable
{
    private readonly CancellationTokenSource _stoppingCts = new();
    private Task _executingTask;

    public virtual void Dispose()
    {
        _stoppingCts.Cancel();
    }

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        _executingTask = ExecuteAsync(_stoppingCts.Token);

        if (_executingTask.IsCompleted)
        {
            return _executingTask;
        }

        return Task.CompletedTask;
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask == null)
        {
            return;
        }

        try
        {
            _stoppingCts.Cancel();
        }
        finally
        {
            await Task.WhenAny(_executingTask,
                               Task.Delay(Timeout.Infinite,
                                          cancellationToken));
        }
    }

    protected abstract Task ExecuteAsync(CancellationToken stoppingToken);
}




