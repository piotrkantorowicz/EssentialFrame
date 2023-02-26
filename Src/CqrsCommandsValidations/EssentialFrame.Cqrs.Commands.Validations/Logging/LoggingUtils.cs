using Microsoft.Extensions.Logging;

namespace EssentialFrame.Cqrs.Commands.Validations.Logging;

internal static class LoggingUtils
{
    internal static EventId ValidationFailed { get; } = new(2004, "ValidationFailed");

    internal static EventId ValidationSuccess { get; } = new(2005, "ValidationSuccess");

    internal static EventId UnexpectedException { get; } = new(2006, "UnexpectedException");
}