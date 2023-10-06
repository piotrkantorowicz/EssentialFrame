using EssentialFrame.Background;

namespace EssentialFrame.Cqrs.Commands.Services.Scheduling.Base;

internal abstract class DefaultCommandSchedulerBase : BackgroundService
{
    private readonly int? _timeInterval;

    protected DefaultCommandSchedulerBase(int? timeInterval)
    {
        _timeInterval = timeInterval;
    }

  
}