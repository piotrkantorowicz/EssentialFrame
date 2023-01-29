using Microsoft.Extensions.Logging;

namespace EssentialFrame.Cqrs.Events.Logging;

public class LoggingUtils
{
    internal static EventId EventExecuting { get; } = new(2000, "EventExecuting");

    internal static EventId EventExecuted { get; } = new(2001, "EventExecuted");

    internal static EventId UnexpectedException { get; } = new(2006, "UnexpectedException");
}