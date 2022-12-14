namespace EssentialFrame.BackgroundTasks.Interfaces;

public interface IBackgroundService
{
    public Task StartAsync(CancellationToken cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken);
}
