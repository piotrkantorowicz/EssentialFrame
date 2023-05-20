using System.Threading;
using System.Threading.Tasks;

namespace EssentialFrame.Background.Interfaces;

public interface IBackgroundService
{
    public Task StartAsync(CancellationToken cancellationToken);
    
    public Task StopAsync(CancellationToken cancellationToken);
}